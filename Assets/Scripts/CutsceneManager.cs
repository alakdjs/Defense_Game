using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _dialogueText;

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

        ShowCurrent();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Next();
        }
    }

    private void ShowCurrent()
    {
        if (_cutsceneData == null || _cutsceneData.frames == null || _cutsceneData.frames.Length == 0)
        {
            SceneManager.LoadScene(_nextSceneName);
            return;
        }

        if (_frameIndex >= _cutsceneData.frames.Length)
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
