using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour
{
    [SerializeField] private float _collectRange = 3.0f;
    [SerializeField] private LayerMask _expLayer;

    private PlayerLevel _playerLevel;

    private void Awake()
    {
        _playerLevel = GetComponent<PlayerLevel>();
    }

    private void Update()
    {
        CollectNearbyExp();
    }

    private void CollectNearbyExp()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _collectRange, _expLayer);

        foreach (var hit in hits)
        {
            ExpBall ball = hit.GetComponent<ExpBall>();
            if (ball != null)
            {
                ball.StartAttract(transform, _playerLevel);
            }
        }
    }

}
