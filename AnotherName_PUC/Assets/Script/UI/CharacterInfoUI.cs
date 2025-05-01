using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CharacterInfoUI : MonoBehaviour
{
    [Header("탭 패널들")]
    public GameObject panelBasicInfo;
    public GameObject panelSkills;
    public GameObject panelAwakenedSkills;

    [Header("기본 선택될 버튼")]
    public HoverTextColor defaultTabButton;

    [Header("캐릭터 기본 정보 UI")]
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    private void OnEnable()
    {
        ShowBasicInfo();
        ApplyCharacterInfo(); // 캐릭터 정보 적용
        StartCoroutine(DelayedSelectDefaultTab());
    }

    private IEnumerator DelayedSelectDefaultTab()
    {
        yield return null; // 한 프레임 기다림

        if (defaultTabButton != null && SelectionManager.Instance != null)
        {
            SelectionManager.Instance.ForceSelectDefault(defaultTabButton);
        }
    }

    private void ApplyCharacterInfo()
    {
        var info = SelectedCharacterData.Instance?.selectedCharacter;
        if (info == null)
        {
            Debug.LogWarning("[CharacterInfoUI] 선택된 캐릭터 정보가 없습니다.");
            return;
        }

        if (portraitImage != null)
            portraitImage.sprite = info.portraitSprite;

        if (nameText != null)
            nameText.text = info.characterName;

        if (levelText != null)
            levelText.text = "Lv.1"; // 필요 시 실제 캐릭터 레벨 정보로 대체
    }

    public void ShowBasicInfo()
    {
        panelBasicInfo.SetActive(true);
        panelSkills.SetActive(false);
        panelAwakenedSkills.SetActive(false);
    }

    public void ShowSkills()
    {
        panelBasicInfo.SetActive(false);
        panelSkills.SetActive(true);
        panelAwakenedSkills.SetActive(false);
    }

    public void ShowAwakenedSkills()
    {
        panelBasicInfo.SetActive(false);
        panelSkills.SetActive(false);
        panelAwakenedSkills.SetActive(true);
    }
}