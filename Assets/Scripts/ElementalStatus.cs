using UnityEngine;

/// <summary>
/// 이 컴포넌트를 가진 오브젝트만 속성 상성 적용
/// </summary>
public class ElementalStatus : MonoBehaviour
{
    [Header("Element")]
    [SerializeField] private ElementType _element = ElementType.Normal;

    public ElementType Element => _element;

    /// <summary>
    /// 외부에서 속성 변경(무기 장착 시 플레이어 속성 변경)
    /// </summary>
    public void SetElement(ElementType element)
    {
        _element = element;
    }
}
