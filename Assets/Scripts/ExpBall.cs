using UnityEngine;


public class ExpBall : MonoBehaviour
{
    [SerializeField] private int _expAmount = 2;
    [SerializeField] private float _followSpeed = 12.0f;

    private bool _isAttracting = false;
    private Transform _target;
    private PlayerLevel _playerLevel;


    // 플레이어에게 흡수
    public void StartAttract(Transform target, PlayerLevel level)
    {
        if (_isAttracting)
            return;

        _isAttracting = true;
        _target = target;
        _playerLevel = level;
    }

    private void Update()
    {
        if (!_isAttracting || _target == null)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            _target.position,
            Time.deltaTime * _followSpeed
            );

        if (Vector3.SqrMagnitude(transform.position - _target.position) < 0.05f)
        {
            Absorb();
        }
    }

    private void Absorb()
    {
        if (_playerLevel != null)
        {
            _playerLevel.AddExp(_expAmount);
        }

        Destroy(gameObject);
    }

}
