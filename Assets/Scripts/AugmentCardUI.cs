using UnityEngine;
using UnityEngine.UI;

public class AugmentCardUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _exText;
    [SerializeField] private Button _selectButton;

    private AugmentData _augmentData;

    public void Init(AugmentData data, int currentLevel = 0)
    {
        _augmentData = data;

        if (_titleText != null)
        {
            if (currentLevel > 0)
                _titleText.text = $"{data.augmentName} Lv.{currentLevel + 1}";
            else
                _titleText.text = data.augmentName;
        }

        // 아이콘 세팅
        if (_iconImage != null)
        {
            _iconImage.sprite = data.icon;
            _iconImage.enabled = data.icon != null;
        }

        // 제목 세팅
        if (_titleText != null)
        {
            _titleText.text = data.augmentName;
        }

        // 설명 세팅
        if (_exText != null)
        {
            int nextLevel = currentLevel > 0 ? currentLevel + 1 : 1;
            _exText.text = data.GetDescription(nextLevel - 1);
        }

        // 버튼 이벤트 중복 방지
        _selectButton.onClick.RemoveAllListeners();
        _selectButton.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        // 안전 체크
        if (_augmentData == null)
        {
            Debug.LogWarning("BuildUpCardUI : AugmentData가 없습니다.");
            return;
        }

        // 증강 선택 이벤트 전달
        AugmentManager.Instance.ApplyAugment(_augmentData);

        // 증강 선택 UI 닫기 요청
        AugmentPopupController.Instance.ClosePopup();
    }
}
