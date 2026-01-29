using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private Image _pauseButtonImage;
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Sprite _playSprite;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleStateChanged;
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

        if (isPaused)
        {
            _pauseButtonImage.sprite = _playSprite;
        }
        else
        {
            _pauseButtonImage.sprite = _pauseSprite;
        }

    }

}
