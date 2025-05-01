using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text targetText;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;

    public CharacterInfoUI characterInfoUI; // CharacterInfoUI 연결
    public enum TabType { BasicInfo, Skills, AwakenedSkills }
    public TabType tabType;

    private bool isSelected = false;

    private void Start()
    {
        if (targetText != null)
            targetText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && targetText != null)
            targetText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && targetText != null)
            targetText.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.SelectButton(this);

            isSelected = true;
            if (targetText != null)
                targetText.color = selectedColor;

            // 탭 패널 전환
            if (characterInfoUI != null)
            {
                switch (tabType)
                {
                    case TabType.BasicInfo:
                        characterInfoUI.ShowBasicInfo();
                        break;
                    case TabType.Skills:
                        characterInfoUI.ShowSkills();
                        break;
                    case TabType.AwakenedSkills:
                        characterInfoUI.ShowAwakenedSkills();
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"[HoverTextColor] CharacterInfoUI가 연결되어 있지 않습니다: {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning("[HoverTextColor] SelectionManager 인스턴스가 없습니다.");
        }
    }

    public void Deselect()
    {
        isSelected = false;
        if (targetText != null)
            targetText.color = normalColor;
    }

    // ✅ 추가: 강제 선택 색상 적용
    public void ForceSelectColor()
    {
        isSelected = true;
        if (targetText != null)
            targetText.color = selectedColor;
    }
}