using UnityEngine;

public class GameOverPopupController : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPopupRoot;
    [SerializeField] private SceneLoader _sceneLoader;

    private void Awake()
    {
        // 초기 상태에서 팝업이 켜져있지 않도록
        if (_gameOverPopupRoot != null)
            _gameOverPopupRoot.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleStateChanged;

        // Enable 타이밍에 이미 Result일 수도 있어서 한 번 동기화
        SyncUIWithState();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState oldState, GameState newState)
    {
        bool isGameOver = (newState == GameState.Result);

        // 팝업 토글
        if (_gameOverPopupRoot != null)
            _gameOverPopupRoot.SetActive(isGameOver);
    }

    private void SyncUIWithState()
    {
        if (GameManager.Instance == null)
            return;

        bool isGameOver = (GameManager.Instance.CurrentState == GameState.Result);

        if (_gameOverPopupRoot != null)
            _gameOverPopupRoot.SetActive(isGameOver);
    }

    public void OnClickRestart()
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("[GameOverPopupController] SceneLoader가 연결되지 않았습니다.");
            return;
        }

        _sceneLoader.RestartCurrentScene();
    }

    public void OnClickGoToTitle()
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("[GameOverPopupController] SceneLoader가 연결되지 않았습니다.");
            return;
        }

        _sceneLoader.GoStartScene();
    }

    public void OnClickQuit()
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("[GameOverPopupController] SceneLoader가 연결되지 않았습니다.");
            return;
        }

        _sceneLoader.QuitGame();
    }

}
