using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void SetTitleStateIfPossible()
    {
        // Title로 이동할 때 런 플래그/카운트 리셋
        if (GameManager.Instance != null)
            GameManager.Instance.SetState(GameState.Title);
    }

    public void GoStartScene()
    {
        ResetTimeScale();
        SetTitleStateIfPossible();
        LoadingManager.Instance.LoadSceneAsync("StartScene");
    }

    public void GoCutsceneScene()
    {
        ResetTimeScale();
        LoadingManager.Instance.LoadSceneAsync("CutsceneScene");
    }

    public void GoSampleScene()
    {
        ResetTimeScale();
        LoadingManager.Instance.LoadSceneAsync("SampleScene");
    }

    // 현재 씬 재시작
    public void RestartCurrentScene()
    {
        ResetTimeScale();

        if (GameManager.Instance != null)
            GameManager.Instance.SetState(GameState.Playing);

        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadingManager.Instance.LoadSceneAsync(currentSceneName);
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
