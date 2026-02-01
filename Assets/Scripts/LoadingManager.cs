using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("Overlay Prefab")]
    [SerializeField] private GameObject _loadingOverlayPrefab;

    private GameObject _overlayInstance;
    private LoadingOverlayView _view;

    // 중복 로드 방지
    private bool _isLoading = false;

    private void Awake()
    {
        // 싱글톤 + 씬 유지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 로딩 오버레이를 띄우고, 비동기로 씬 로드
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        // 로딩 중이면 중복 호출 방지
        if (_isLoading)
            return;

        StartCoroutine(CoLoadSceneAsync(sceneName));
    }

    private IEnumerator CoLoadSceneAsync(string sceneName)
    {
        _isLoading = true;

        ShowOverlay();

        if (_view != null)
            _view.SetProgress(0f);

        // UI가 한 프레임 그려질 시간을 확보
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // progress는 0.9까지 올라가고, allowSceneActivation에서 진짜 넘어감
        while (op.progress < 0.9f)
        {
            // 0~0.9 → 0~1로 정규화
            float normalizedProgress = Mathf.Clamp01(op.progress / 0.9f);

            if (_view != null)
                _view.SetProgress(normalizedProgress);

            yield return null;
        }

        // 100% 채우기
        if (_view != null)
            _view.SetProgress(1f);

        // 다음 프레임에 씬 활성화
        yield return null;

        op.allowSceneActivation = true;

        // 씬 전환 후 한 프레임 기다렸다가 오버레이 숨김
        yield return null;

        HideOverlay();

        _isLoading = false;
    }

    private void ShowOverlay()
    {
        if (_loadingOverlayPrefab == null)
            return;

        if (_overlayInstance == null)
        {
            _overlayInstance = Instantiate(_loadingOverlayPrefab);
            DontDestroyOnLoad(_overlayInstance);

            _view = _overlayInstance.GetComponentInChildren<LoadingOverlayView>(true);
        }

        _overlayInstance.SetActive(true);
    }

    private void HideOverlay()
    {
        if (_overlayInstance != null)
        {
            _overlayInstance.SetActive(false);
        }
    }
}
