using System;
using UnityEngine;

public enum GameState
{
    Title,         // 메인화면(타이틀)
    Settings,
    Cutscene,      // 컷씬
    Playing,       // 인게임 진행
    Paused,        // 게임 일시정지
    AugmentSelect, // 증강 선택(레벨업)
    Result         // 게임 종료 후 결과
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<GameState, GameState> OnGameStateChanged;

    [SerializeField] private GameState _currentState = GameState.Title;
    public GameState CurrentState => _currentState;
    private GameState _prevState = GameState.Title;

    private void Awake()
    {
        // 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }

    private void HandleEscapeKey()
    {
        switch (_currentState)
        {
            case GameState.Playing:
                TogglePause();
                break;

            case GameState.Paused:
                TogglePause();
                break;

            case GameState.AugmentSelect:
                // 증강 선택 중 ESC는 무시
                return;

            default:
                return;
        }
    }

    public void SetState(GameState nextState)
    {
        if (_currentState == nextState)
            return;

        GameState old = _currentState;
        _prevState = old;
        _currentState = nextState;

        ApplyGlobalPolicy(_currentState);

        Debug.Log($"[GameManager] State Changed: {old} -> {_currentState}");
        OnGameStateChanged?.Invoke(old, _currentState);
    }

    private void ApplyGlobalPolicy(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
            case GameState.Settings:
            case GameState.Result:
            case GameState.AugmentSelect:
                Time.timeScale = 0f;
                break;

            case GameState.Cutscene:
                Time.timeScale = 1f;
                break;
        }
    }

    public void TogglePause()
    {
        if (_currentState == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
        else if (_currentState == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }

    /// <summary>
    /// 게임 결과 처리
    /// </summary>
    public void GameOver()
    {
        SetState(GameState.Result);
    }

    /// <summary>
    /// 컷씬 시작/종료
    /// </summary>
    public void StartCutscene()
    {
        SetState(GameState.Cutscene);
    }

    public void EndCutscene()
    {
        // 컷신 끝나면 이전 상태로 복귀하거나 Playing으로 고정
        SetState(GameState.Playing);
    }
}
