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
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneAsync(string sceneName)
    {
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

        // UI 먼저 표시
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float displayedProgress = 0f;
        float minDisplayTime = 0.5f;   // 너무 빨리 끝나도 최소한 잠깐은 보이게
        float elapsed = 0f;

        while (op.progress < 0.9f)
        {
            elapsed += Time.unscaledDeltaTime;

            // Unity progress(0~0.9)를 0~0.99로 정규화
            float realProgress = Mathf.Clamp01(op.progress / 0.9f) * 0.99f;

            // 실제 진행률을 너무 늦지 않게 따라가되 약간만 부드럽게
            displayedProgress = Mathf.Lerp(displayedProgress, realProgress, Time.unscaledDeltaTime * 8f);

            // 너무 느리게 끌려가지 않도록 하한 보정
            if (realProgress - displayedProgress < 0.01f)
                displayedProgress = realProgress;

            if (_view != null)
                _view.SetProgress(displayedProgress);

            yield return null;
        }

        // 실제 로딩은 끝났지만, 너무 빨리 끝났다면 최소 표시 시간만 맞춤
        while (elapsed < minDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;

            displayedProgress = Mathf.Lerp(displayedProgress, 0.99f, Time.unscaledDeltaTime * 10f);

            if (_view != null)
                _view.SetProgress(displayedProgress);

            yield return null;
        }

        // 마지막 100%
        if (_view != null)
            _view.SetProgress(1f);

        yield return new WaitForSecondsRealtime(0.1f);

        op.allowSceneActivation = true;

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
            _overlayInstance.SetActive(false);
    }
}