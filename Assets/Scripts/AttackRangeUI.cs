using UnityEngine;

public class AttackRangeUI : MonoBehaviour
{
    [SerializeField] private Transform _visual;
    [SerializeField] private float _scaleMultiplier = 2.0f;
    [SerializeField] private bool _alwaysVisible = true;

    private void Awake()
    {
        if (_visual == null)
            _visual = transform;
    }

    // 공격 범위 반영
    public void SetRange(float range)
    {
        float diameter = range * _scaleMultiplier;
        _visual.localScale = new Vector3(diameter, diameter, 1.0f);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

}
