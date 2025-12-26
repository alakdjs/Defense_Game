using UnityEngine;
using UnityEngine.UI;


public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _hpText;

    private float _maxHp;

    public void Init(float maxHp)
    {
        _maxHp = maxHp;
        SetHp(maxHp);
    }

    public void SetHp(float currentHp)
    {
        float ratio = currentHp / _maxHp;
        _fillImage.fillAmount = ratio;

        _hpText.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(_maxHp)}";
    }
}
