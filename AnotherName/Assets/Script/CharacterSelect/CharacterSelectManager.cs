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

    [Header("프리팹 분리")]
    public GameObject[] characterPreviewPrefabs;   // 사용 안 해도 무방
    public GameObject[] characterPlayablePrefabs;  // 인게임용

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

        if (characterPreviewPrefabs.Length > index && characterPreviewPrefabs[index] != null)
        {
            currentPreview = Instantiate(characterPreviewPrefabs[index], previewRoot.position, Quaternion.identity, previewRoot);
            currentPreview.transform.localScale = Vector3.one * 2f;
        }
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

        // ✅ 씬 내 기존 Player 제거
        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null)
        {
            Destroy(existingPlayer);
            Debug.Log("[선택] 기존 씬 Player 오브젝트 제거 완료");
        }

        // ✅ 이전 선택된 프리팹 제거
        if (SelectedCharacterData.Instance.selectedCharacterPrefab != null)
        {
            Destroy(SelectedCharacterData.Instance.selectedCharacterPrefab);
            SelectedCharacterData.Instance.selectedCharacterPrefab = null;
            Debug.Log("[선택] selectedCharacterPrefab 제거 완료");
        }

        // ✅ 새 캐릭터 프리팹 인스턴스화
        GameObject selectedPlayablePrefab = characterPlayablePrefabs[selectedIndex];
        GameObject persistentPrefab = Instantiate(selectedPlayablePrefab);
        DontDestroyOnLoad(persistentPrefab);

        string prefabName = selectedPlayablePrefab.name;

        // 자동 캐릭터 이름 부여
        string autoName = characterNames[selectedIndex];
        switch (prefabName)
        {
            case "Player1": autoName = "불에 삼켜진 자"; break;
            case "Player2": autoName = "하늘에서 온 자"; break;
            case "Player3": autoName = "다가설 수 없는 자"; break;
        }

        // ✅ 선택 캐릭터 데이터 저장
        SelectedCharacterData.Instance.SelectCharacter(
            characterIds[selectedIndex], autoName, characterPortraits[selectedIndex], passiveSkillIcons[selectedIndex],
            skillRIcons[selectedIndex], skillEIcons[selectedIndex], skillQIcons[selectedIndex],
            skillQDescriptions[selectedIndex], skillEDescriptions[selectedIndex], skillRDescriptions[selectedIndex],
            passiveSkillNames[selectedIndex], passiveSkillDescriptions[selectedIndex], passiveSkillLevels[selectedIndex],
            persistentPrefab,
            prefabName
        );

        SceneManager.LoadScene("VillageScene");
    }
}