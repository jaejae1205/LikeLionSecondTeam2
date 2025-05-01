using UnityEngine;

[CreateAssetMenu(fileName = "PortalSpawnData", menuName = "Spawn/Portal Spawn Data")]
public class PortalSpawnData : ScriptableObject
{
    public string portalId;
    public string sceneName;
    public Vector3 spawnPosition;
}