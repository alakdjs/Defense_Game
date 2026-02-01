using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingOverlayView : MonoBehaviour
{
    [Header("Progress UI")]
    [SerializeField] private Image _progressFillImage;
    [SerializeField] private TMP_Text _progressText;

    public void SetProgress(float normalized01)
    {
        if (_progressFillImage != null)
            _progressFillImage.fillAmount = normalized01;

        if (_progressText != null)
            _progressText.text = $"{Mathf.RoundToInt(normalized01 * 100f)}%";
    }
}
