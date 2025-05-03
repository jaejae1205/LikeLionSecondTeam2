using UnityEngine;

[System.Serializable]
public class CharacterInfo
{
    public string characterId;
    public string characterName;
    public Sprite portraitSprite;
    public Sprite passiveSkillIcon;

    public Sprite skillRIcon;
    public Sprite skillEIcon;
    public Sprite skillQIcon;

    public string skillQDescription;
    public string skillEDescription;
    public string skillRDescription;

    public string passiveSkillName;
    public string passiveSkillDescription;
    public int passiveSkillLevel;
}

public class SelectedCharacterData : MonoBehaviour
{
    public static SelectedCharacterData Instance { get; private set; }

    public CharacterInfo selectedCharacter;

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

    public void SelectCharacter(
        string id, string name, Sprite portrait, Sprite passiveIcon,
        Sprite skillR, Sprite skillE, Sprite skillQ,
        string descQ, string descE, string descR,
        string passiveName, string passiveDesc, int passiveLevel)
    {
        selectedCharacter = new CharacterInfo
        {
            characterId = id,
            characterName = name,
            portraitSprite = portrait,
            passiveSkillIcon = passiveIcon,
            skillRIcon = skillR,
            skillEIcon = skillE,
            skillQIcon = skillQ,
            skillQDescription = descQ,
            skillEDescription = descE,
            skillRDescription = descR,
            passiveSkillName = passiveName,
            passiveSkillDescription = passiveDesc,
            passiveSkillLevel = passiveLevel
        };
    }
}