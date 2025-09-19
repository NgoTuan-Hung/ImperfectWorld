#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FloatModifier))]
public class FloatModifierInspector : PropertyDrawer
{
    [SerializeField]
    private VisualTreeAsset floatModifierAsset;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        if (floatModifierAsset != null)
            root.Add(floatModifierAsset.CloneTree());
        return root;
    }
}

#endif
