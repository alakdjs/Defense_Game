using UnityEngine;
using UnityEngine.UI;


public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _levelText;

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
