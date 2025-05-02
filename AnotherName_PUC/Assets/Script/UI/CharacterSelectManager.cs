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

    [Header("프리팹 & 프리뷰")]
    public GameObject[] characterPrefabs;
    public Transform previewRoot;
    private GameObject currentPreview;

    [Header("캐릭터 데이터")]
    public string[] characterIds;           // 예: "fire_walker", "sky_striker"
    public string[] characterNames;         // 예: "불에 삼켜진 자", "하늘에서 온 자"
    public Sprite[] characterPortraits;     // 얼굴 이미지 (UI에 표시될)

    [Header("효과음")]
    public AudioClip selectSfx;
    public AudioClip confirmSfx;

    [Header("패시브 스킬 아이콘")]
    public Sprite[] passiveSkillIcons;

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

        // 🔊 캐릭터 선택 효과음
        if (AudioManager.Instance != null && selectSfx != null)
        {
            AudioManager.Instance.PlaySfx(selectSfx);
        }

        // 설명 출력
        if (index >= 0 && index < characterDescriptions.Length)
        {
            descriptionText.text = characterDescriptions[index];
        }

        // 프리뷰 변경
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
            {
                descriptionText.text = "<color=#FFCC00>캐릭터를 선택하세요!</color>";
            }
            return;
        }

        StartCoroutine(ConfirmSelectionDelayed());
    }

    private IEnumerator ConfirmSelectionDelayed()
    {
        if (AudioManager.Instance != null && confirmSfx != null)
        {
            AudioManager.Instance.PlaySfx(confirmSfx);
        }

        yield return new WaitForSeconds(0.3f);

        string id = characterIds[selectedIndex];
        string name = characterNames[selectedIndex];
        Sprite portrait = characterPortraits[selectedIndex];
        Sprite passiveIcon = passiveSkillIcons[selectedIndex];

        Sprite skillR = skillRIcons[selectedIndex];
        Sprite skillE = skillEIcons[selectedIndex];
        Sprite skillQ = skillQIcons[selectedIndex];

        SelectedCharacterData.Instance.SelectCharacter(id, name, portrait, passiveIcon, skillR, skillE, skillQ);

        SceneManager.LoadScene("VillageScene");
    }
}