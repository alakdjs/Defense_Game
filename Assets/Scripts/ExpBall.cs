using System.Collections;
using UnityEngine;


public class ExpBall : MonoBehaviour
{
    [SerializeField] private int _expAmount = 5;
    [SerializeField] private float _followSpeed = 12.0f;

    private float _blinkStartTime = 10.0f; // 10초 후 깜빡임 시작
    private float _despawnTime = 20.0f;    // 20초 후 사라짐
    private float _blinkInterval = 0.15f;  // 깜빡임 속도


    private bool _isAttracting = false;
    private Transform _target;
    private PlayerLevel _playerLevel;

    // 깜빡임 제어용
    private Renderer[] _renderers;
    private Coroutine _lifeCoroutine;
    private bool _isDespawned = false;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void OnEnable()
    {
        // 생성되자마자 수명 타이머 시작
        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);

        _lifeCoroutine = StartCoroutine(Co_LifeCycle());
    }

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

    private IEnumerator Co_LifeCycle()
    {
        // 10초까지 대기
        if (_blinkStartTime > 0.0f)
            yield return new WaitForSeconds(_blinkStartTime);

        // 10초 ~ 20초: 깜빡임
        float remain = Mathf.Max(0.0f, _despawnTime - _blinkStartTime);
        float t = 0.0f;
        bool visible = true;

        while (t < remain)
        {
            if (_isDespawned) yield break;

            visible = !visible;
            SetVisible(visible);

            float wait = Mathf.Max(0.01f, _blinkInterval);
            yield return new WaitForSeconds(wait);
            t += wait;
        }

        // 20초: 사라짐(흡수 못했을 때)
        Despawn();
    }

    private void SetVisible(bool visible)
    {
        if (_renderers != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                    _renderers[i].enabled = visible;
            }
        }
    }

    private void Despawn()
    {
        if (_isDespawned) return;
        _isDespawned = true;

        Destroy(gameObject);
    }

    private void Absorb()
    {
        if (_isDespawned)
            return;

        _isDespawned = true;

        if (_playerLevel != null)
        {
            _playerLevel.AddExp(_expAmount);
        }

        Destroy(gameObject);
    }
}
