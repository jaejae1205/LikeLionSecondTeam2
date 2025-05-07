using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public GameObject menuUI;
    public GameObject optionUI;

    [Header("캐릭터 정보 팝업")]
    public GameObject characterInfoUI;

    [Header("체력 UI")]
    public Image hpBarFillImage;

    // ✅ 체력바 점멸 관련 변수
    private Coroutine hpFlashCoroutine;
    private Color originalHpColor = Color.green;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindUIReferences();

        if (SelectedCharacterData.Instance != null)
        {
            var info = SelectedCharacterData.Instance.selectedCharacter;
            ApplyCharacterInfo(info);
        }

        // ✅ 체력바 원래 색상 기억
        if (hpBarFillImage != null)
            originalHpColor = hpBarFillImage.color;
    }

    public void RebindUIReferences()
    {
        if (menuUI == null)
            menuUI = GameObject.Find("MenuUI");

        if (optionUI == null)
            optionUI = GameObject.Find("OptionUI");

        if (characterInfoUI == null)
            characterInfoUI = GameObject.Find("CharacterInfoUI");

        if (portraitImage == null)
            portraitImage = GameObject.Find("Portrait")?.GetComponent<Image>();

        if (nameText == null)
            nameText = GameObject.Find("NameText")?.GetComponent<TextMeshProUGUI>();

        if (levelText == null)
            levelText = GameObject.Find("LevelText")?.GetComponent<TextMeshProUGUI>();

        if (passiveSkillIconImage == null)
            passiveSkillIconImage = GameObject.Find("PassiveSkillIcon")?.GetComponent<Image>();

        if (skillRIconImage == null)
            skillRIconImage = GameObject.Find("SkillR")?.GetComponent<Image>();

        if (skillEIconImage == null)
            skillEIconImage = GameObject.Find("SkillE")?.GetComponent<Image>();

        if (skillQIconImage == null)
            skillQIconImage = GameObject.Find("SkillQ")?.GetComponent<Image>();

        if (hpBarFillImage == null)
            hpBarFillImage = GameObject.Find("HpBarFill")?.GetComponent<Image>();
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

    public void ShowMenuPopup()
    {
        if (menuUI != null)
            menuUI.SetActive(true);
        else
            Debug.LogWarning("[InGameUIManager] menuUI가 연결되지 않았습니다.");
    }

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
        SceneManager.LoadScene("TitleScene");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ✅ 체력 업데이트 + 투명도 점멸 효과
    public void UpdateHpUI(int currentHp, int maxHp)
    {
        if (hpBarFillImage == null)
        {
            Debug.LogWarning("[UI] hpBarFillImage가 null입니다. Fill 오브젝트 연결 필요.");
            return;
        }

        float fillAmount = Mathf.Clamp01((float)currentHp / maxHp);
        hpBarFillImage.fillAmount = fillAmount;

        // 점멸 효과 실행 (투명도)
        if (hpFlashCoroutine != null)
            StopCoroutine(hpFlashCoroutine);
        hpFlashCoroutine = StartCoroutine(FlashHpBarFade());
    }

    private IEnumerator FlashHpBarFade()
    {
        Color fadedColor = originalHpColor;
        fadedColor.a = 0.3f; // 투명도 낮춤
        hpBarFillImage.color = fadedColor;

        yield return new WaitForSeconds(0.1f);

        hpBarFillImage.color = originalHpColor;
    }
}