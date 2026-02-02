using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _hpText;

    private float _maxHp;

    public void Init(float maxHp)
    {
        _maxHp = maxHp;
        SetHp(maxHp);
    }

    public void SetHp(float currentHp)
    {
        currentHp = Mathf.Clamp(currentHp, 0f, _maxHp);

        float ratio = currentHp / _maxHp;
        _fillImage.fillAmount = ratio;

        _hpText.text = $"{currentHp:F2} / {_maxHp:F2}";
    }
}
