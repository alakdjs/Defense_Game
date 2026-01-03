using UnityEngine;

public class TowerMain : MonoBehaviour
{
    [SerializeField] private float _petRadius = 5.0f; // 펫(서브타워) 이동 반경

    public float PetRadius => _petRadius;

    public void OnTowerDestroyed()
    {
        // Game Over 처리
    }
}
