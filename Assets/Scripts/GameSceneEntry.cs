using UnityEngine;

public class GameSceneEntry : MonoBehaviour
{
    [SerializeField] private bool _setPlayingOnStart = true;

    private void Start()
    {
        if (!_setPlayingOnStart)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[GameSceneEntry] GameManager가 없습니다. (GameStart 씬에서 DontDestroyOnLoad로 생성됐는지 확인)");
            return;
        }

        GameManager.Instance.SetState(GameState.Playing);
    }
}
