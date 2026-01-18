using UnityEngine;
using System.Collections.Generic;


public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance;

    [SerializeField] private List<WeaponData> _weaponDataList;

    private Dictionary<(WeaponType, WeaponElementType), WeaponData> _weaponDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildDatabase();
    }

    private void BuildDatabase()
    {
        _weaponDict = new Dictionary<(WeaponType, WeaponElementType), WeaponData>();

        foreach (var data in _weaponDataList)
        {
            if (data == null)
                continue;

            var key = (data.WeaponType, data.ElementType);

            if (_weaponDict.ContainsKey(key))
            {
                Debug.LogError($"[WeaponDatabase] 중복 무기: {data.WeaponType} + {data.ElementType}");
                continue;
            }

            _weaponDict.Add(key, data);
        }
    }

    // 무기 조회
    public WeaponData GetWeapon(WeaponType type, WeaponElementType element)
    {
        if (_weaponDict.TryGetValue((type, element), out var weapon))
            return weapon;

        Debug.LogError($"[WeaponDatabase] 무기 없음: {type} + {element}");
        return null;
    }
}
