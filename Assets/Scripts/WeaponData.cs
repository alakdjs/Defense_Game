using UnityEngine;

public enum WeaponType
{
    Sword = 0,
    Rifle = 1
}

// 무기 속성
public enum WeaponElementType
{
    WoodStick = 0,
    Normal = 1,
    Fire = 2,
    Electric = 3,
    Water = 4,
    Rock = 5,
    Ice = 6
}

[CreateAssetMenu(menuName = "Weapon/WeaponData")]

public class WeaponData : ScriptableObject
{
    public WeaponType WeaponType; // 무기 종류
    public WeaponElementType ElementType; // 무기 속성
    public GameObject WeaponPrefab; // 실제 무기 프리팹
    public float Damage = 50.0f; // 데미지
    public float AttackRange = 2.0f; // 공격 인식 범위
}
