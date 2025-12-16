using UnityEngine;

public class MonsterDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            Die();
        }
    }

    void Die()
    {
        // 나중에 애니메이션, 이펙트 추가 가능
        Destroy(gameObject);
    }
}
