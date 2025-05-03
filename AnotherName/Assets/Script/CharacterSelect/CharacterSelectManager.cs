using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text descriptionText;

    [Header("캐릭터 설명")]
    [TextArea] public string[] characterDescriptions;

    [Header("스킬 설명 (Q/E/R 순)")]
    [TextArea] public string[] skillQDescriptions;
    [TextArea] public string[] skillEDescriptions;
    [TextArea] public string[] skillRDescriptions;

    [Header("프리팹 & 프리뷰")]
    public GameObject[] characterPrefabs;
    public Transform previewRoot;
    private GameObject currentPreview;

    [Header("캐릭터 데이터")]
    public string[] characterIds;
    public string[] characterNames;
    public Sprite[] characterPortraits;

    [Header("효과음")]
    public AudioClip selectSfx;
    public AudioClip confirmSfx;

    [Header("패시브 스킬 아이콘")]
    public Sprite[] passiveSkillIcons;

    [Header("패시브 스킬 정보")]
    public string[] passiveSkillNames;
    [TextArea] public string[] passiveSkillDescriptions;
    public int[] passiveSkillLevels;

    [Header("스킬 아이콘")]
    public Sprite[] skillRIcons;
    public Sprite[] skillEIcons;
    public Sprite[] skillQIcons;

    private int selectedIndex = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    public void SelectCharacter(int index)
    {
        selectedIndex = index;

        if (AudioManager.Instance != null && selectSfx != null)
        {
            AudioManager.Instance.PlaySfx(selectSfx);
        }

        if (index >= 0 && index < characterDescriptions.Length)
        {
            descriptionText.text = characterDescriptions[index];
        }

        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(characterPrefabs[index], previewRoot.position, Quaternion.identity, previewRoot);
        currentPreview.transform.localScale = Vector3.one * 2f;
    }

    public void ConfirmSelection()
    {
        if (selectedIndex == -1)
        {
            if (descriptionText != null)
                descriptionText.text = "<color=#FFCC00>캐릭터를 선택하세요!</color>";
            return;
        }

        StartCoroutine(ConfirmSelectionDelayed());
    }

    private IEnumerator ConfirmSelectionDelayed()
    {
        if (AudioManager.Instance != null && confirmSfx != null)
            AudioManager.Instance.PlaySfx(confirmSfx);

        yield return new WaitForSeconds(0.3f);

        string id = characterIds[selectedIndex];
        string name = characterNames[selectedIndex];
        Sprite portrait = characterPortraits[selectedIndex];
        Sprite passiveIcon = passiveSkillIcons[selectedIndex];
        Sprite skillR = skillRIcons[selectedIndex];
        Sprite skillE = skillEIcons[selectedIndex];
        Sprite skillQ = skillQIcons[selectedIndex];

        string descQ = skillQDescriptions[selectedIndex];
        string descE = skillEDescriptions[selectedIndex];
        string descR = skillRDescriptions[selectedIndex];

        string passiveName = passiveSkillNames[selectedIndex];
        string passiveDesc = passiveSkillDescriptions[selectedIndex];
        int passiveLevel = passiveSkillLevels[selectedIndex];

        SelectedCharacterData.Instance.SelectCharacter(
            id, name, portrait, passiveIcon,
            skillR, skillE, skillQ,
            descQ, descE, descR,
            passiveName, passiveDesc, passiveLevel
        );

        SceneManager.LoadScene("VillageScene");
    }
}