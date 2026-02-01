using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private Image _pauseButtonImage;
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Sprite _playSprite;

    [SerializeField] private GameObject _pausePopupRoot;
    [SerializeField] private Image _dimBackgroundImage;

    [SerializeField] private SceneLoader _sceneLoader;

    private void Awake()
    {
        // 초기 상태에서 팝업이 켜져있지 않도록
        if (_pausePopupRoot != null)
            _pausePopupRoot.SetActive(false);

    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleStateChanged;

        // Enable 타이밍에 이미 상태가 Paused일 수도 있어서 한 번 동기화
        SyncUIWithState();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    public void TogglePause()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.TogglePause();
    }

    private void HandleStateChanged(GameState oldState, GameState newState)
    {
        bool isPaused = (newState == GameState.Paused || newState == GameState.Settings);

        // 버튼 스프라이트 변경
        if (_pauseButtonImage != null)
            _pauseButtonImage.sprite = isPaused ? _playSprite : _pauseSprite;

        // 팝업 토글
        if (_pausePopupRoot != null)
            _pausePopupRoot.SetActive(isPaused);

    }

    private void SyncUIWithState()
    {
        if (GameManager.Instance == null)
            return;

        GameState state = GameManager.Instance.CurrentState;
        bool isPaused = (state == GameState.Paused || state == GameState.Settings);

        // 버튼 스프라이트 동기화
        if (_pauseButtonImage != null)
            _pauseButtonImage.sprite = isPaused ? _playSprite : _pauseSprite;

        // 팝업 동기화
        if (_pausePopupRoot != null)
            _pausePopupRoot.SetActive(isPaused);
    }

    public void OnClickGoToTitle()
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("[PauseController] SceneLoader가 연결되지 않았습니다.");
            return;
        }

        _sceneLoader.GoStartScene();
    }

    public void OnClickQuit()
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("[PauseController] SceneLoader가 연결되지 않았습니다.");
            return;
        }

        _sceneLoader.QuitGame();
    }

}
