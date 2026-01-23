public static class ElementalCombat
{
    public const float Strong = 1.5f;
    public const float Weak = 0.5f;
    public const float Normal = 1.0f;

    /// <summary>
    /// 타입 배율
    /// </summary>
    public static float GetMultiplier(ElementType attacker, ElementType defender)
    {
        // 둘 중 하나라도 Normal이면 상성 없음
        if (attacker == ElementType.Normal || defender == ElementType.Normal)
            return Normal;

        // 불꽃: 얼음 1.5, 물 0.5
        if (attacker == ElementType.Fire)
        {
            if (defender == ElementType.Ice) return Strong;
            if (defender == ElementType.Water) return Weak;
            return Normal;
        }

        // 전기: 물 1.5, 전기 0.5
        if (attacker == ElementType.Electric)
        {
            if (defender == ElementType.Water) return Strong;
            if (defender == ElementType.Electric) return Weak;
            return Normal;
        }

        // 물: 불꽃 1.5, 얼음 0.5
        if (attacker == ElementType.Water)
        {
            if (defender == ElementType.Fire) return Strong;
            if (defender == ElementType.Ice) return Weak;
            return Normal;
        }

        // 바위: 전기 1.5, 바위 0.5
        if (attacker == ElementType.Rock)
        {
            if (defender == ElementType.Electric) return Strong;
            if (defender == ElementType.Rock) return Weak;
            return Normal;
        }

        // 얼음: 물 1.5, 불꽃 0.5
        if (attacker == ElementType.Ice)
        {
            if (defender == ElementType.Water) return Strong;
            if (defender == ElementType.Fire) return Weak;
            return Normal;
        }

        return Normal;
    }

    /// <summary>
    /// WeaponElementType -> ElementType 변환
    /// </summary>
    public static ElementType ToElementType(WeaponElementType weaponElement)
    {
        switch (weaponElement)
        {
            case WeaponElementType.Fire: return ElementType.Fire;
            case WeaponElementType.Electric: return ElementType.Electric;
            case WeaponElementType.Water: return ElementType.Water;
            case WeaponElementType.Rock: return ElementType.Rock;
            case WeaponElementType.Ice: return ElementType.Ice;
            case WeaponElementType.WoodStick: return ElementType.Normal;
            case WeaponElementType.Normal: return ElementType.Normal;
            default: return ElementType.Normal;
        }
    }
}
