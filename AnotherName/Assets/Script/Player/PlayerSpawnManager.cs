using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class PlayerSpawnManager : MonoBehaviour
{
    public PortalDatabase portalDatabase;

    private GameObject player;
    private Rigidbody2D playerRb;
    private CinemachineCamera virtualCamera;

    private void Start()
    {
        StartCoroutine(WaitForCameraAndSpawn());
    }

    private IEnumerator WaitForCameraAndSpawn()
    {
        LoadingUIManager.Instance?.ShowLoading(); // 로딩 UI 표시

        float waitTime = 0f;
        float timeout = 3f;

        while (virtualCamera == null && waitTime < timeout)
        {
            var cameras = Object.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
            if (cameras.Length > 1)
            {
                for (int i = 1; i < cameras.Length; i++)
                {
                    Destroy(cameras[i].gameObject);
                    Debug.Log("[스폰] 중복 CinemachineCamera 제거 완료");
                }

                virtualCamera = cameras[0];
            }
            else if (cameras.Length == 1)
            {
                virtualCamera = cameras[0];
                DontDestroyOnLoad(virtualCamera.gameObject);
                Debug.Log("[스폰] CinemachineCamera 최초 인식 및 DontDestroyOnLoad 처리 완료");
            }

            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (virtualCamera == null)
            Debug.LogWarning("[스폰] CinemachineCamera를 찾지 못했습니다. 스폰은 계속 진행됩니다.");

        yield return StartCoroutine(ApplySpawn());

        yield return new WaitForSeconds(0.2f);
        LoadingUIManager.Instance?.HideLoading(); // 로딩 UI 종료
    }

    private IEnumerator ApplySpawn()
    {
        yield return null;

        float timer = 0f;
        while ((player == null || !player.activeInHierarchy || player.GetComponent<Rigidbody2D>() == null) && timer < 2f)
        {
            player = GameObject.FindWithTag("Player");
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // ✅ 선택된 캐릭터 프리팹 연결 (DontDestroyOnLoad 된 객체)
        if (player == null && SelectedCharacterData.Instance != null)
        {
            GameObject prefab = SelectedCharacterData.Instance.selectedCharacterPrefab;
            if (prefab != null)
            {
                player = prefab;
                player.tag = "Player";
                Debug.Log("[스폰] 선택된 캐릭터 프리팹 연결 완료");
            }
        }

        if (player == null || portalDatabase == null)
        {
            Debug.LogWarning("[스폰] Player 또는 PortalDatabase 연결 실패");
            yield break;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        string lastPortalId = SceneLoadData.Instance?.LastPortalName;

        if (SceneLoadData.Instance != null && SceneLoadData.Instance.IsVillageStartRequired())
        {
            lastPortalId = "VillageStart";
            SceneLoadData.Instance.EnteredFromGameStart = false;
            Debug.Log("[게임시작] VillageScene → VillageStart 위치 사용");
        }

        var spawnData = portalDatabase.GetSpawnData(lastPortalId, currentScene);
        if (spawnData != null)
        {
            Vector3 spawnPos = spawnData.spawnPosition;

            playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                yield return new WaitForFixedUpdate();
                playerRb.MovePosition(spawnPos);
            }

            player.transform.position = spawnPos;

            if (virtualCamera != null)
            {
                if (virtualCamera.Follow != player.transform)
                {
                    virtualCamera.Follow = player.transform;
                    Debug.Log("[스폰] CinemachineCamera의 Follow 대상 설정됨");
                }

                if (virtualCamera.LookAt != player.transform)
                {
                    virtualCamera.LookAt = player.transform;
                    Debug.Log("[스폰] CinemachineCamera의 LookAt 대상도 설정됨");
                }

                // ✅ Confiner 자동 설정
                var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
                if (confiner != null)
                {
                    GameObject groundObject = GameObject.FindWithTag("Ground");
                    if (groundObject != null)
                    {
                        Collider2D groundCollider = groundObject.GetComponent<Collider2D>();
                        if (groundCollider != null)
                        {
                            confiner.BoundingShape2D = groundCollider;
                            confiner.InvalidateBoundingShapeCache();
                            Debug.Log("[카메라] Confiner에 Ground Collider 자동 설정 완료");
                        }
                        else
                        {
                            Debug.LogWarning("[카메라] Ground 오브젝트에 Collider2D가 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[카메라] 'Ground' 태그를 가진 오브젝트를 찾지 못했습니다.");
                    }
                }
            }
            else
            {
                StartCoroutine(WaitUntilCameraFoundAndAssignFollow());
            }

            Debug.Log($"[스폰 완료] 위치 = {player.transform.position}, 활성 상태 = {player.activeSelf}");
        }
        else
        {
            Debug.LogWarning($"[스폰] SpawnData 매칭 실패 → PortalId: {lastPortalId}, Scene: {currentScene}");
        }

        yield return null;

        foreach (var portal in Object.FindObjectsByType<StagePortal>(FindObjectsSortMode.None))
        {
            portal.EnablePortalAfterSpawn();
            Debug.Log($"[포탈] EnablePortalAfterSpawn 호출 → {portal.portalName}");
        }

        yield return new WaitForSeconds(0.2f);
        if (SceneLoadData.Instance != null)
        {
            SceneLoadData.Instance.LastPortalName = null;
            Debug.Log("[스폰] LastPortalName 초기화 완료");
        }
    }

    private IEnumerator WaitUntilCameraFoundAndAssignFollow()
    {
        float waitTime = 0f;
        float maxWait = 3f;

        while (virtualCamera == null && waitTime < maxWait)
        {
            var found = Object.FindFirstObjectByType<CinemachineCamera>();
            if (found != null)
            {
                virtualCamera = found;
                SceneManager.MoveGameObjectToScene(virtualCamera.gameObject, SceneManager.GetActiveScene());

                virtualCamera.Follow = player?.transform;
                virtualCamera.LookAt = player?.transform;

                // ✅ Confiner도 재설정
                var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
                if (confiner != null)
                {
                    GameObject groundObject = GameObject.FindWithTag("Ground");
                    if (groundObject != null)
                    {
                        Collider2D groundCollider = groundObject.GetComponent<Collider2D>();
                        if (groundCollider != null)
                        {
                            confiner.BoundingShape2D = groundCollider;
                            confiner.InvalidateBoundingShapeCache();
                            Debug.Log("[카메라] 재설정된 Confiner에 Ground Collider 자동 적용");
                        }
                    }
                }

                virtualCamera.gameObject.SetActive(false);
                yield return null;
                virtualCamera.gameObject.SetActive(true);

                Debug.Log("[스폰] 나중에 등장한 CinemachineCamera에 Follow 및 Confiner 재설정 완료");
                yield break;
            }

            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (virtualCamera == null)
        {
            Debug.LogWarning("[스폰] 일정 시간 내 카메라 찾기 실패 → Follow 설정 불가");
        }
    }
}