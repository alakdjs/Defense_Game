using TMPro;
using UnityEngine;

public class GiftBoxCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _countText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGiftBoxCountChanged += UpdateUI;

        // 활성화 시 현재 값 동기화
        Sync();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGiftBoxCountChanged -= UpdateUI;
    }

    private void Sync()
    {
        if (GameManager.Instance == null)
            return;

        UpdateUI(GameManager.Instance.GiftBoxCount);
    }

    private void UpdateUI(int count)
    {
        if (_countText == null)
            return;

        _countText.text = $"획득한 선물상자: {count}개";
    }
}
