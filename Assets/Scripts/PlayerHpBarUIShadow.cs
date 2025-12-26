using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PlayerHpBar의 Background (테두리) 오브젝트에 넣음
/// </summary>
public class PlayerHpBarUIShadow : MonoBehaviour
{
    [SerializeField] private Shadow[] _shadows;
    [SerializeField] private Outline _outline;

    [SerializeField] private float _blinkSpeed = 6.0f;
    [SerializeField] private float _dangerHpRatio = 0.2f; // 체력이 20% 남았을 때

    private bool _isDanger = false;

    public void SetDanger(bool isDanger)
    {
        _isDanger = isDanger;
    }

    private void Update()
    {
        if (!_isDanger)
        {
            SetColor(Color.white);
            return;
        }
        float t = Mathf.PingPong(Time.unscaledTime * _blinkSpeed, 1.0f);
        Color dangerColor = Color.Lerp(Color.white, Color.red, t);

        SetColor(dangerColor);
    }

    private void SetColor(Color baseColor)
    {
        if (_outline != null)
        {
            _outline.effectColor = new Color(baseColor.r, baseColor.g, baseColor.b, _outline.effectColor.a);
        }

        foreach (var s in _shadows)
        {
            s.effectColor = new Color(baseColor.r, baseColor.g, baseColor.b, s.effectColor.a);
        }

    }

}
