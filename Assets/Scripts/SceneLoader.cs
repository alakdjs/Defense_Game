using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public void GoSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    // 게임 종료 버튼용
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 에디터에서 종료 테스트용
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임 종료
        Application.Quit();
#endif
    }

}
