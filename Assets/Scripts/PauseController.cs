using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private Image _pauseButtonImage;
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Sprite _playSprite;

    private bool _isPaused = false;

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            _pauseButtonImage.sprite = _playSprite;
        }
        else
        {
            Time.timeScale = 1f;
            _pauseButtonImage.sprite = _pauseSprite;
        }
    }

}
