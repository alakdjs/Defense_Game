using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IsoCamera : MonoBehaviour
{
    public Transform _target;
    public Vector3 _offset = new Vector3(0, 15, -10);
    public float _followSpeed = 15.0f;

    private float _shakeDuration = 0.08f; // 흔들림 시간
    private float _shakeStrength = 0.25f; // 흔들림 강도

    private Vector3 _shakeOffset;
    private Coroutine _shakeCoroutine;

    void LateUpdate()
    {
        if (_target == null) return;
        
        Vector3 desiredPosition = _target.position + _offset + _shakeOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, _followSpeed * Time.deltaTime);

        //transform.LookAt(_target);
        transform.rotation = Quaternion.Euler(55, 0, 0);
    }

    public void Shake()
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private System.Collections.IEnumerator ShakeRoutine()
    {
        float time = 0f;

        while (time < _shakeDuration)
        {
            time += Time.deltaTime;

            _shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ) * _shakeStrength;

            yield return null;
        }

        _shakeOffset = Vector3.zero;
    }
}
