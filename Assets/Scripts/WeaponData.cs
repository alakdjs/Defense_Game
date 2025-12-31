using UnityEngine;

public enum WeaponType
{
    Sword = 0,
    Rifle = 1
}

[CreateAssetMenu(menuName = "Weapon/WeaponData")]

public class WeaponData : ScriptableObject
{
    public WeaponType _weaponType; // 무기 타입
    public GameObject _weaponPrefab; // 실제 무기 프리팹
    public float _damage = 30.0f; // 데미지
    public float _attackRange = 2.0f; // 공격 인식 범위
}
