using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleUIManager : MonoBehaviour
{
    [Header("공용 버튼 클릭 사운드")]
    public AudioClip buttonClickSfx;

    /// <summary>
    /// 버튼 클릭 사운드 재생
    /// </summary>
    private void PlayClickSound()
    {
        if (AudioManager.Instance != null && buttonClickSfx != null)
        {
            AudioManager.Instance.PlaySfx(buttonClickSfx);
        }
        else
        {
            Debug.LogWarning("[TitleUIManager] 효과음 재생 실패: AudioManager 또는 AudioClip 누락");
        }
    }

    public void OnClickGameStart()
    {
        if (SceneLoadData.Instance != null)
        {
            SceneLoadData.Instance.EnteredFromGameStart = true;
            Debug.Log("[타이틀] 게임 시작 → EnteredFromGameStart = true");
        }

        StartCoroutine(GameStartDelayed());
    }

    private IEnumerator GameStartDelayed()
    {
        PlayClickSound();
        yield return new WaitForSeconds(0.3f); // 사운드 출력 시간 확보
        SceneManager.LoadScene("OpeningScene");
    }

    public void OnClickOption()
    {
        PlayClickSound();

        if (OptionManager.Instance == null)
        {
            Debug.LogError("[TitleUIManager] OptionManager.Instance가 null입니다!");
        }
        else
        {
            Debug.Log("OptionManager 호출 성공");
            OptionManager.Instance.ShowOption();
        }
    }

    public void OnClickExit()
    {
        StartCoroutine(ExitDelayed());
    }

    private IEnumerator ExitDelayed()
    {
        PlayClickSound();
        yield return new WaitForSeconds(0.3f); // 사운드 출력 시간 확보
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}