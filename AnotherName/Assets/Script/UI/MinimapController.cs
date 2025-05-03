using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public RectTransform minimapRect;
    public RectTransform playerIcon;
    public GameObject portalIconPrefab;
    public PortalDatabase portalDatabase;

    private Transform player;
    private float mapRadiusWorld = 8f;
    private float mapUIRadius;
    private List<PortalSpawnData> portalData = new();
    private List<GameObject> portalIcons = new();

    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null);
        player = GameObject.FindWithTag("Player").transform;

        mapUIRadius = minimapRect.sizeDelta.x / 2f;
        LoadPortalData();
        CreatePortalIcons();
    }

    void LoadPortalData()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        portalData = portalDatabase.spawnPoints.FindAll(p => p.sceneName == currentScene);
    }

    void CreatePortalIcons()
    {
        foreach (var portal in portalData)
        {
            GameObject icon = Instantiate(portalIconPrefab, minimapRect);
            icon.SetActive(true);
            portalIcons.Add(icon);
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ✅ 1. 플레이어는 중앙 고정
        playerIcon.anchoredPosition = Vector2.zero;
        playerIcon.localRotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);

        // ✅ 2. 포탈 아이콘 위치 갱신
        for (int i = 0; i < portalData.Count; i++)
        {
            var data = portalData[i];

            // 🎯 VillageStart는 예외로 표시하지 않음
            if (data.portalId == "VillageStart")
            {
                portalIcons[i].SetActive(false);
                continue;
            }

            Vector2 offset = data.spawnPosition - player.position;

            if (offset.magnitude > mapRadiusWorld)
            {
                portalIcons[i].SetActive(false);
                continue;
            }

            Vector2 iconPos = (offset / mapRadiusWorld) * mapUIRadius;
            portalIcons[i].SetActive(true);
            portalIcons[i].GetComponent<RectTransform>().anchoredPosition = iconPos;
        }
    }
}