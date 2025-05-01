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

    private void Awake()
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
            Debug.Log("[스폰] CinemachineCamera 최초 생성 및 DontDestroyOnLoad 처리 완료");
        }
        else
        {
            Debug.LogWarning("[스폰] CinemachineCamera를 찾지 못했습니다.");
        }
    }

    private void Start()
    {
        StartCoroutine(ApplySpawn());
    }

    private IEnumerator ApplySpawn()
    {
        yield return null;

        float timer = 0f;
        while ((player == null || !player.activeInHierarchy || player.GetComponent<Rigidbody2D>() == null) && timer < 2f)
        {
            player = GameObject.FindWithTag("Player");
            timer += Time.deltaTime;
            yield return null;
        }

        if (player == null || portalDatabase == null)
        {
            Debug.LogWarning("[스폰] Player 또는 PortalDatabase 연결 실패");
            yield break;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        string lastPortalId = SceneLoadData.Instance?.LastPortalName;

        // ▶ 명시적으로 "게임 시작 → VillageScene" 진입인 경우
        if (SceneLoadData.Instance != null && SceneLoadData.Instance.IsVillageStartRequired())
        {
            lastPortalId = "VillageStart";
            SceneLoadData.Instance.EnteredFromGameStart = false; // 한 번만 사용
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
}