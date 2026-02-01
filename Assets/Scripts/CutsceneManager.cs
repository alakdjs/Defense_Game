using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _dialogueText;

    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;

    [Header("Data")]
    [SerializeField] private CutsceneData _cutsceneData;
    [SerializeField] private string _nextSceneName;

    private int _frameIndex = 0;
    private int _dialogueIndex = 0;

    private void Start()
    {
        // 컷씬 진입 상태로 전환
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartCutscene();
        }

        if (_prevButton != null)
            _prevButton.onClick.AddListener(OnClickPrev);

        if (_nextButton != null)
            _nextButton.onClick.AddListener(OnClickNext);

        ShowCurrent();
        RefreshButtonState();
    }

    private void Update()
    {
        // 다음
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Next();
        }

        // 이전
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Prev();
        }
    }

    public void OnClickNext()
    {
        Next();
    }

    public void OnClickPrev()
    {
        Prev();
    }

    private void ShowCurrent()
    {
        if (_cutsceneData == null || _cutsceneData.frames == null || _cutsceneData.frames.Length == 0)
        {
            SceneManager.LoadScene(_nextSceneName);
            return;
        }

        CutsceneFrame frame = _cutsceneData.frames[_frameIndex];

        if (_background != null)
        {
            _background.sprite = frame.image;
        }

        if (_dialogueText != null)
        {
            _dialogueText.text = GetDialogue(frame, _dialogueIndex);
        }
    }

    private void Next()
    {
        CutsceneFrame frame = _cutsceneData.frames[_frameIndex];
        int dialogueCount = frame.dialogues != null ? frame.dialogues.Length : 0;

        // 대사가 더 남아 있으면 대사만 넘김
        if (dialogueCount > 0 && _dialogueIndex < dialogueCount - 1)
        {
            _dialogueIndex++;
            ShowCurrent();
            RefreshButtonState();
            return;
        }

        // 이 컷 끝 → 다음 컷
        _frameIndex++;
        _dialogueIndex = 0;

        if (_frameIndex >= _cutsceneData.frames.Length)
        {
            SceneManager.LoadScene(_nextSceneName);
            return;
        }

        ShowCurrent();
        RefreshButtonState();
    }

    private void Prev()
    {
        // 같은 컷에서 이전 대사가 있으면
        if (_dialogueIndex > 0)
        {
            _dialogueIndex--;
            ShowCurrent();
            RefreshButtonState();
            return;
        }

        // 첫 컷 + 첫 대사면 더 이상 못 감
        if (_frameIndex == 0)
        {
            RefreshButtonState();
            return;
        }

        // 이전 컷으로 이동
        _frameIndex--;

        CutsceneFrame prevFrame = _cutsceneData.frames[_frameIndex];
        int dialogueCount = prevFrame.dialogues != null ? prevFrame.dialogues.Length : 0;

        // 이전 컷의 마지막 대사로
        _dialogueIndex = Mathf.Max(0, dialogueCount - 1);

        ShowCurrent();
        RefreshButtonState();
    }

    private void RefreshButtonState()
    {
        // Prev: 첫 컷 + 첫 대사면 비활성
        if (_prevButton != null)
        {
            bool canPrev = !(_frameIndex == 0 && _dialogueIndex == 0);
            _prevButton.interactable = canPrev;
        }

        if (_nextButton != null)
        {
            _nextButton.interactable = true;
        }
    }


    private string GetDialogue(CutsceneFrame frame, int index)
    {
        if (frame.dialogues == null || frame.dialogues.Length == 0)
            return string.Empty;

        if (index < 0 || index >= frame.dialogues.Length)
            return string.Empty;

        return frame.dialogues[index];
    }
}
