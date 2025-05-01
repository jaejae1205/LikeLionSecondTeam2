using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PortalDatabase", menuName = "Spawn/Portal Database")]
public class PortalDatabase : ScriptableObject
{
    public List<PortalSpawnData> spawnPoints;

    public PortalSpawnData GetSpawnData(string portalId, string sceneName)
    {
        Debug.Log($"[비교 기준] 요청된 portalId: \"{portalId}\", sceneName: \"{sceneName}\"");

        foreach (var data in spawnPoints)
        {
            if (data == null)
            {
                Debug.LogWarning("[경고] PortalDatabase에 null 항목이 존재합니다!");
                continue;
            }

            Debug.Log($"[등록된 데이터] portalId: \"{data.portalId}\", sceneName: \"{data.sceneName}\"");
        }

        // 최종 Trim 보정된 비교
        return spawnPoints.FirstOrDefault(p =>
            p != null &&
            p.portalId != null && p.sceneName != null &&
            p.portalId.Trim() == portalId.Trim() &&
            p.sceneName.Trim() == sceneName.Trim());
    }

}