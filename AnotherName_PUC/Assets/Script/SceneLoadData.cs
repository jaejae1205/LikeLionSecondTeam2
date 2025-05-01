using UnityEngine;

public class SceneLoadData : MonoBehaviour
{
    public static SceneLoadData Instance;

    public string LastPortalName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }
}