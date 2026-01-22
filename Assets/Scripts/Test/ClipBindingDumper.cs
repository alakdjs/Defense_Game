#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ClipBindingDumper : EditorWindow
{
    private AnimationClip _clip;

    [MenuItem("Tools/Animation/Clip Binding Dumper")]
    private static void Open()
    {
        GetWindow<ClipBindingDumper>("Clip Binding Dumper");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Animation Clip에 저장된 경로(Path)들을 출력합니다.");
        _clip = (AnimationClip)EditorGUILayout.ObjectField("Clip", _clip, typeof(AnimationClip), false);

        if (_clip == null)
            return;

        if (GUILayout.Button("Dump Bindings To Console"))
        {
            Dump(_clip);
        }
    }

    private static void Dump(AnimationClip clip)
    {
        Debug.Log($"[ClipBindingDumper] Clip = {clip.name}");

        var bindings = AnimationUtility.GetCurveBindings(clip);

        // 너무 많을 수 있으니 상위 몇 개만 먼저 출력
        int max = Mathf.Min(bindings.Length, 60);

        for (int i = 0; i < max; i++)
        {
            var b = bindings[i];
            Debug.Log($"[{i}] path='{b.path}', type={b.type.Name}, property='{b.propertyName}'");
        }

        Debug.Log($"[ClipBindingDumper] Total Bindings = {bindings.Length}");
    }
}
#endif
