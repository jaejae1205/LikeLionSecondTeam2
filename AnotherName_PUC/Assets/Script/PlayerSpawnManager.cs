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
        LoadingUIManager.Instance?.ShowLoading(); // 로딩 페이지 표시

        float waitTime = 0f;
        float timeout = 3f;

        while (virtualCamera == null && waitTime < timeout)
        {
            var existingCameras = Object.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
            if (existingCameras.Length > 1)
            {
                for (int i = 1; i < existingCameras.Length; i++)
                {
                    Destroy(existingCameras[i].gameObject);
                    Debug.Log("[스폰] 중복 CinemachineCamera 제거 완료");
                }
                virtualCamera = existingCameras[0];
            }
            else if (existingCameras.Length == 1)
            {
                virtualCamera = existingCameras[0];
                DontDestroyOnLoad(virtualCamera.gameObject);
                Debug.Log("[스폰] CinemachineCamera 최초 인식 및 DontDestroyOnLoad 처리 완료");
            }

            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (virtualCamera == null)
        {
            Debug.LogWarning("[스폰] CinemachineCamera를 찾지 못했습니다. 스폰은 계속 진행됩니다.");
        }

        yield return StartCoroutine(ApplySpawn());

        yield return new WaitForSeconds(0.2f);
        LoadingUIManager.Instance?.HideLoading(); // 로딩 페이지 종료
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
            Debug.Log("[게임시작] 게임 시작으로 VillageScene 진입 → VillageStart 위치 사용");
        }

        var spawnData = portalDatabase.GetSpawnData(lastPortalId, currentScene);

        if (spawnData != null)
        {
            Vector3 spawnPos = spawnData.spawnPosition;
            Debug.Log($"[스폰] 위치 저장됨: {spawnPos}");

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
                virtualCamera.Follow = player.transform;
                Debug.Log("[스폰] CinemachineCamera의 Follow 대상이 플레이어로 설정됨");
            }
            else
            {
                StartCoroutine(WaitUntilCameraFoundAndAssignFollow());
            }

            Debug.Log($"[스폰 최종] transform = {player.transform.position}, rb = {playerRb?.position}, active = {player.activeSelf}");
        }
        else
        {
            Debug.LogWarning($"[스폰] SpawnData 매칭 실패 → PortalId: {lastPortalId}, Scene: {currentScene}");
        }

        yield return null;

        foreach (var portal in Object.FindObjectsByType<StagePortal>(FindObjectsSortMode.None))
        {
            portal.EnablePortalAfterSpawn();
            Debug.Log($"[포탈] EnablePortalAfterSpawn 호출됨 → {portal.portalName}");
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

                // ✅ 현재 씬으로 귀속시켜 카메라 꼬임 방지
                SceneManager.MoveGameObjectToScene(virtualCamera.gameObject, SceneManager.GetActiveScene());

                // ✅ Follow 대상 재설정
                virtualCamera.Follow = player?.transform;

                // ✅ 카메라 강제 리셋 (일부 컴포넌트 문제 예방)
                virtualCamera.gameObject.SetActive(false);
                yield return null;
                virtualCamera.gameObject.SetActive(true);

                Debug.Log("[스폰] 나중에 등장한 CinemachineCamera에 Follow 재설정 + 씬 재귀속 완료");
                yield break;
            }

            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (virtualCamera == null)
        {
            Debug.LogWarning("[스폰] 일정 시간 내에 CinemachineCamera를 찾지 못해 Follow 설정 실패");
        }
    }
}