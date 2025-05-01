using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    public PortalDatabase portalDatabase;

    private GameObject player;
    private Vector3? pendingSpawnPos = null;
    private Rigidbody2D playerRb;

    private void Start()
    {
        StartCoroutine(ApplySpawn());
    }

    private IEnumerator ApplySpawn()
    {
        yield return null;

        // ✅ Player가 완전히 활성화될 때까지 대기 (DontDestroyOnLoad 구조 대응)
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

        string lastPortalId = SceneLoadData.Instance?.LastPortalName;
        string currentScene = SceneManager.GetActiveScene().name;

#if UNITY_EDITOR
        if (string.IsNullOrEmpty(lastPortalId))
        {
            lastPortalId = "ToStageA"; // 디버그용
            Debug.Log("[디버그] LastPortalId가 비어 있어 'ToStageA'로 강제 설정됨");
        }
#endif

        var spawnData = portalDatabase.GetSpawnData(lastPortalId, currentScene);

        if (spawnData != null)
        {
            Vector3 spawnPos = spawnData.spawnPosition;
            Debug.Log($"[스폰] 위치 저장됨: {spawnPos}");

            playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                yield return new WaitForFixedUpdate();  // ✅ 물리 프레임 이후에 반영
                playerRb.MovePosition(spawnPos);
            }

            player.transform.position = spawnPos;

            if (Camera.main != null)
                Camera.main.transform.position = new Vector3(spawnPos.x, spawnPos.y, Camera.main.transform.position.z);

            Debug.Log($"[스폰 최종] transform = {player.transform.position}, rb = {playerRb?.position}, active = {player.activeSelf}");
        }
        else
        {
            Debug.LogWarning($"[스폰] SpawnData 매칭 실패 → PortalId: {lastPortalId}, Scene: {currentScene}");
        }

        // ✅ 포탈 콜라이더 활성화
        foreach (var portal in FindObjectsByType<StagePortal>(FindObjectsSortMode.None))
        {
            portal.EnablePortalAfterSpawn();
        }

        // ✅ 스폰 위치가 적용된 이후에만 초기화
        yield return new WaitForSeconds(0.2f);
        if (SceneLoadData.Instance != null)
        {
            SceneLoadData.Instance.LastPortalName = null;
            Debug.Log("[스폰] LastPortalName 초기화 완료");
        }
    }
}