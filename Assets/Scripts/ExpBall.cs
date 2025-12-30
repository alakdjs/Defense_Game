using UnityEngine;

public class ExpBall : MonoBehaviour
{
    [SerializeField] private int _expAmount = 2;
    [SerializeField] private float _rotateSpeed = 90.0f;

    private void Update()
    {
        transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerLevel level = other.GetComponent<PlayerLevel>();
        if (level != null)
        {
            level.AddExp(_expAmount);
        }

        Destroy(gameObject);
    }
}
