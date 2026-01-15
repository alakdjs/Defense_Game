using UnityEngine;

public class AttackRangeUI : MonoBehaviour
{
    [SerializeField] private Transform _visual;
    [SerializeField] private float _scaleMultiplier = 1.0f;

    private float _baseWorldSizeX = 1.0f; // scale=1일 때 월드 X 크기(지름 기준)
    private float _baseWorldSizeY = 1.0f; // scale=1일 때 월드 Y 크기

    private void Awake()
    {
        if (_visual == null)
            _visual = transform;

        CacheBaseWorldSize();
    }

    public void SetRange(float range)
    {
        float desiredDiameter = range * 2.0f * _scaleMultiplier;

        // 현재 오브젝트가 scale=1일 때 월드에서 차지하는 크기를 기준으로 스케일 계산
        float sx = desiredDiameter / _baseWorldSizeX;
        float sy = desiredDiameter / _baseWorldSizeY;

        _visual.localScale = new Vector3(sx, sy, _visual.localScale.z);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    // scale=1일 때 월드에서 실제 크기가 얼마인지 캐시
    private void CacheBaseWorldSize()
    {
        var renderer = _visual.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Vector3 originalScale = _visual.localScale;

            _visual.localScale = Vector3.one;
            Vector3 size = renderer.bounds.size;

            _baseWorldSizeX = Mathf.Max(0.0001f, size.x);
            _baseWorldSizeY = Mathf.Max(0.0001f, size.z);

            _visual.localScale = originalScale;
            return;
        }

        _baseWorldSizeX = 1.0f;
        _baseWorldSizeY = 1.0f;
    }
}
