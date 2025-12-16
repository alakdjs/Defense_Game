using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IsoCamera : MonoBehaviour
{
    public Transform _target;
    public Vector3 _offset = new Vector3(0, 10, -10);
    public float _followSpeed = 10.0f;

    void LateUpdate()
    {
        if (_target == null) return;
        
        Vector3 desiredPosition = _target.position + _offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, _followSpeed * Time.deltaTime);

        //transform.LookAt(_target);
        transform.rotation = Quaternion.Euler(45, 0, 0);
    }
}
