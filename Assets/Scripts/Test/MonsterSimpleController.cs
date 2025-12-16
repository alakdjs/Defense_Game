using UnityEngine;

public class MonsterSimpleController : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;

    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || target == null) return;

        // 플레이어 쪽으로 이동
        Vector3 dir = (target.position - transform.position);
        dir.y = 0;

        if (dir.magnitude > 0.1f)
        {
            transform.position += dir.normalized * moveSpeed * Time.deltaTime;
            transform.forward = dir.normalized;

            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Attack"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");

        Destroy(gameObject, 1.5f); // 애니메이션 후 제거
    }
}
