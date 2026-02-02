using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _levelText;

    public void SetExp(float currentExp, float maxExp, int level)
    {
        float ratio = (float)currentExp / maxExp;
        _fillImage.fillAmount = ratio;

        if (_levelText != null)
        {
            _levelText.text = $"{level}";
        }
    }
}
