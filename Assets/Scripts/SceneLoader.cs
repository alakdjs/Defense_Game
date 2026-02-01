using UnityEngine;


public class SceneLoader : MonoBehaviour
{
    public void GoStartScene()
    {
        LoadingManager.Instance.LoadSceneAsync("StartScene");
    }

    public void GoCutsceneScene()
    {
        LoadingManager.Instance.LoadSceneAsync("CutsceneScene");
    }

    public void GoSampleScene()
    {
        LoadingManager.Instance.LoadSceneAsync("SampleScene");
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
