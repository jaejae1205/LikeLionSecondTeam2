using UnityEngine;

[System.Serializable]
public class CharacterInfo
{
    public string characterId;
    public string characterName;
    public Sprite portraitSprite;
    public Sprite passiveSkillIcon;

    public Sprite skillRIcon;   // R스킬 아이콘
    public Sprite skillEIcon;   // E스킬 아이콘
    public Sprite skillQIcon;   // Q스킬 아이콘
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

    public void SelectCharacter(string id, string name, Sprite portrait, Sprite passiveIcon, Sprite skillR, Sprite skillE, Sprite skillQ)
    {
        selectedCharacter = new CharacterInfo
        {
            characterId = id,
            characterName = name,
            portraitSprite = portrait,
            passiveSkillIcon = passiveIcon,
            skillRIcon = skillR,
            skillEIcon = skillE,
            skillQIcon = skillQ
        };
    }
}