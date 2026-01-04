using UnityEngine;
using System.Collections.Generic;

public class WallTransparencyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _target; // 캐릭터 Transform
    [SerializeField] private Camera _mainCamera;

    [Header("Settings")]
    [SerializeField] private LayerMask _wallLayer; // 벽 레이어
    //[SerializeField] private float _raycastDistance = 100f;

    private List<Material> _transparentMaterials = new List<Material>();
    private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();

    void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    void Update()
    {
        if (_target == null || _mainCamera == null)
            return;

        // 이전에 투명화된 오브젝트들을 원래대로 복원
        RestoreAllMaterials();

        // 카메라에서 타겟으로 레이캐스트
        Vector3 direction = _target.position - _mainCamera.transform.position;
        float distance = Vector3.Distance(_mainCamera.transform.position, _target.position);

        RaycastHit[] hits = Physics.RaycastAll(_mainCamera.transform.position, direction, distance, _wallLayer);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                MakeTransparent(renderer);
            }
        }
    }

    void MakeTransparent(Renderer renderer)
    {
        // 원본 머티리얼 저장 (처음 한 번만)
        if (!_originalMaterials.ContainsKey(renderer))
        {
            _originalMaterials[renderer] = renderer.materials;
        }

        // 각 머티리얼에 투명화 적용
        Material[] mats = renderer.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].HasProperty("_IsTransparent"))
            {
                mats[i].SetFloat("_IsTransparent", 1f);
                mats[i].SetVector("_TargetPosition", _target.position);

                if (!_transparentMaterials.Contains(mats[i]))
                    _transparentMaterials.Add(mats[i]);
            }
        }
        renderer.materials = mats;
    }

    void RestoreAllMaterials()
    {
        // 투명화된 모든 머티리얼 복원
        foreach (Material mat in _transparentMaterials)
        {
            if (mat != null && mat.HasProperty("_IsTransparent"))
            {
                mat.SetFloat("_IsTransparent", 0f);
            }
        }
        _transparentMaterials.Clear();
    }

    void OnDestroy()
    {
        // 씬이 종료될 때 모든 머티리얼 복원
        RestoreAllMaterials();
    }

    // 디버그용 - 레이캐스트 시각화
    void OnDrawGizmos()
    {
        if (_target != null && _mainCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_mainCamera.transform.position, _target.position);
        }
    }
}