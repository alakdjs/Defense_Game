using UnityEngine;

public enum WeaponType
{
    Sword = 0,
    Rifle = 1
}

[CreateAssetMenu(menuName = "Weapon/WeaponData")]

public class WeaponData : ScriptableObject
{
    public WeaponType weaponType; // 무기 타입
    public GameObject weaponPrefab; // 실제 무기 프리팹
}
