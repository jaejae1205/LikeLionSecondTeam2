using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public static OptionManager Instance { get; private set; }

    [Header("옵션 UI 오브젝트")]
    public GameObject optionUIPopup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요하면 사용
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (optionUIPopup == null)
        {
            Debug.Log("[OptionManager] 비활성 포함 탐색 시작");

            // ✅ 비활성 오브젝트까지 포함해서 검색
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("OptionUI") && obj.hideFlags == HideFlags.None)
                {
                    optionUIPopup = obj;
                    Debug.Log("[OptionManager] OptionUI 자동 연결 성공: " + obj.name);
                    break;
                }
            }

            if (optionUIPopup == null)
            {
                Debug.LogError("[OptionManager] 비활성 상태의 OptionUI도 찾지 못했습니다. 태그 확인 또는 하이어라키 존재 여부 확인!");
            }
        }
    }

    /// <summary>
    /// 옵션 UI 팝업 열기
    /// </summary>
    public void ShowOption()
    {
        if (optionUIPopup != null)
        {
            optionUIPopup.SetActive(true);
            Debug.Log("[OptionManager] 옵션 팝업 활성화");
        }
        else
        {
            Debug.LogError("[OptionManager] optionUIPopup이 null입니다. ShowOption 실패");
        }
    }

    /// <summary>
    /// 옵션 UI 팝업 닫기
    /// </summary>
    public void HideOption()
    {
        if (optionUIPopup != null)
        {
            optionUIPopup.SetActive(false);
        }
    }
}