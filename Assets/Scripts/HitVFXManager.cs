using UnityEngine;


public class HitVFXManager : MonoBehaviour
{
    public static HitVFXManager Instance;

    [Header("Sword Hit VFX")]
    [SerializeField] private GameObject _swordHitVfxPrefab;

    [Header("Rifle Hit VFX")]
    [SerializeField] private GameObject _rifleHitVfxPrefab;

    private void Awake()
    {
        // 싱글톤 보호 (중복 방지)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 지정 위치에 Hit VFX 스폰
    /// </summary>
    public void SpawnHitVFX(Vector3 position, WeaponType weaponType)
    {
        GameObject prefab = null;

        if (weaponType == WeaponType.Sword)
        {
            prefab = _swordHitVfxPrefab;
        }
        else if (weaponType == WeaponType.Rifle)
        {
            prefab = _rifleHitVfxPrefab;
        }

        if (prefab == null)
            return;

        Instantiate(prefab, position, Quaternion.identity);

        // 카메라 흔들림
        IsoCamera cam = Camera.main.GetComponent<IsoCamera>();
        if (cam != null)
        {
            cam.Shake();
        }
    }
}
