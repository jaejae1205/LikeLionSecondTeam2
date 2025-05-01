using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 타이틀 이동용

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance { get; private set; }

    [Header("UI 요소")]
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Image passiveSkillIconImage;

    [Header("스킬 아이콘")]
    public Image skillRIconImage;
    public Image skillEIconImage;
    public Image skillQIconImage;

    [Header("메뉴 팝업 UI")]
    public GameObject menuUI;          // 새로 추가: Menu UI 연결
    public GameObject optionUI;        // 기존 Option UI 연결

    [Header("캐릭터 정보 팝업")]
    public GameObject characterInfoUI;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyCharacterInfo(CharacterInfo info)
    {
        if (portraitImage != null)
            portraitImage.sprite = info.portraitSprite;

        if (levelText != null)
            levelText.text = "Lv.1";

        if (nameText != null)
            nameText.text = info.characterName;

        if (passiveSkillIconImage != null)
            passiveSkillIconImage.sprite = info.passiveSkillIcon;

        if (skillRIconImage != null)
            skillRIconImage.sprite = info.skillRIcon;

        if (skillEIconImage != null)
            skillEIconImage.sprite = info.skillEIcon;

        if (skillQIconImage != null)
            skillQIconImage.sprite = info.skillQIcon;
    }

    // 메뉴 버튼 클릭 시
    public void ShowMenuPopup()
    {
        if (menuUI != null)
            menuUI.SetActive(true);
        else
            Debug.LogWarning("[InGameUIManager] menuUI가 연결되지 않았습니다.");
    }

    // 메뉴 팝업 내부 기능들

    public void ResumeGame()
    {
        if (menuUI != null)
            menuUI.SetActive(false);
    }

    public void OpenOptionPopup()
    {
        if (optionUI != null)
        {
            optionUI.SetActive(true);
            menuUI.SetActive(false);
        }
    }
    // 초상화 클릭 시
    public void ShowCharacterInfoPopup()
    {
        if (characterInfoUI != null)
            characterInfoUI.SetActive(true);
        else
            Debug.LogWarning("[InGameUIManager] characterInfoUI가 연결되지 않았습니다.");
    }

    public void CloseCharacterInfoPopup()
    {
        if (characterInfoUI != null)
            characterInfoUI.SetActive(false);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene"); // 돌아가기 클릭 시 타이틀 씬으로 이동
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}