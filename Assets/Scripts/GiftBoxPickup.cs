using UnityEngine;

/// <summary>
/// 플레이어가 선물상자를 먹으면 체력 회복 후 파괴
/// </summary>
public class GiftBoxPickup : MonoBehaviour
{
    [SerializeField] private float _healAmount = 10.0f;
    [SerializeField] private GameObject _pickupVfxPrefab;

    [Tooltip("중복 트리거 방지")]
    [SerializeField] private bool _picked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_picked)
            return;

        // 플레이어 판정
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        _picked = true;

        // 체력 회복
        player.Heal(_healAmount);

        // 이펙트
        if (_pickupVfxPrefab != null)
        {
            GameObject vfx = Instantiate(_pickupVfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2.0f);
        }

        Destroy(gameObject);
    }
}
