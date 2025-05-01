using UnityEngine;
using System.Collections;

public class CharacterInfoUI : MonoBehaviour
{
    [Header("탭 패널들")]
    public GameObject panelBasicInfo;
    public GameObject panelSkills;
    public GameObject panelAwakenedSkills;

    [Header("기본 선택될 버튼")]
    public HoverTextColor defaultTabButton;

    private void OnEnable()
    {
        ShowBasicInfo();
        StartCoroutine(DelayedSelectDefaultTab()); // 🔥 한 프레임 딜레이 실행
    }

    private IEnumerator DelayedSelectDefaultTab()
    {
        yield return null; // 한 프레임 기다림

        if (defaultTabButton != null && SelectionManager.Instance != null)
        {
            SelectionManager.Instance.ForceSelectDefault(defaultTabButton);
        }
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