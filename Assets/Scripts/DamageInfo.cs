using UnityEngine;

public struct DamageInfo
{
    public float damage;
    public ElementType attackerElement;
    public GameObject attacker;

    public DamageInfo(float damage, ElementType attackerElement, GameObject attacker)
    {
        this.damage = damage;
        this.attackerElement = attackerElement;
        this.attacker = attacker;
    }
}
