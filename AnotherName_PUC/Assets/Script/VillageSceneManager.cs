using UnityEngine;

public class VillageSceneManager : MonoBehaviour
{
    
    void Start()
    {
        if (SelectedCharacterData.Instance != null)
        {
            var info = SelectedCharacterData.Instance.selectedCharacter;
            InGameUIManager.Instance.ApplyCharacterInfo(info);
        }
    }

    
    void Update()
    {
        
    }
}
