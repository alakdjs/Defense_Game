using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentCardUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _exText;
    [SerializeField] private Button _selectButton;

    private AugmentData _augmentData;

    public void Init(AugmentData data, int currentStack = 0)
    {
        _augmentData = data;

        // 아이콘
        if (_iconImage != null)
        {
            _iconImage.sprite = data.icon;
            _iconImage.enabled = data.icon != null;
        }

        // 제목
        if (_titleText != null)
        {
            int nextStack = currentStack + 1;
            _titleText.text = $"{data.augmentName}\n(스택 {currentStack} → {nextStack})";
        }

        // 설명
        if (_exText != null)
        {
            _exText.text = data.description;
        }

        // 버튼 이벤트 중복 방지
        _selectButton.onClick.RemoveAllListeners();
        _selectButton.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        if (_augmentData == null)
        {
            Debug.LogWarning("AugmentCardUI : AugmentData가 없습니다.");
            return;
        }

        // 증강 선택 이벤트 전달
        AugmentManager.Instance.ApplyAugment(_augmentData);

        // 증강 선택 UI 닫기 요청
        AugmentPopupController.Instance.ClosePopup();
    }
}
