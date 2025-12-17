using UnityEngine;
using System.Collections.Generic;


public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance;

    [SerializeField] private List<WeaponData> _weaponDataList;

    private Dictionary<WeaponType, List<WeaponData>> _weaponDict;

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

        _weaponDict = new Dictionary<WeaponType, List<WeaponData>>();

        foreach (var data in _weaponDataList)
        {
            if (!_weaponDict.ContainsKey(data.weaponType))
            {
                _weaponDict[data.weaponType] = new List<WeaponData>();
            }

            _weaponDict[data.weaponType].Add(data);
        }

    }

    // 타입에 해당하는 무기 목록 반환
    public List<WeaponData> GetWeaponList(WeaponType type)
    {
        return _weaponDict[type];
    }

    // 기본 무기 (첫 번째)
    public WeaponData GetDefaultWeapon(WeaponType type)
    {
        return _weaponDict[type][0];
    }

    // 랜덤 무기
    public WeaponData GetRandomWeapon(WeaponType type)
    {
        List<WeaponData> list = _weaponDict[type];
        return list[Random.Range(0, list.Count)];
    }

}
