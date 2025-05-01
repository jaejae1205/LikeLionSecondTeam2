using UnityEngine;

public class ButtonSelectionManager : MonoBehaviour
{
    public static ButtonSelectionManager Instance { get; private set; }

    private ButtonColorController selectedButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnButtonClicked(ButtonColorController button)
    {
        if (selectedButton != null && selectedButton != button)
        {
            selectedButton.Deselect();
        }

        selectedButton = button;
        selectedButton.Select();
    }

    public void DeselectAll()
    {
        if (selectedButton != null)
        {
            selectedButton.Deselect();
            selectedButton = null;
        }
    }
}