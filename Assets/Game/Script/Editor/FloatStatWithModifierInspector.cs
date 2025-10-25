#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FloatStatWithModifier))]
public class FloatStatWithModifierInspector : PropertyDrawer
{
    [SerializeField]
    private VisualTreeAsset floatStatWithModifierInspectorAsset;
    FloatField baseValueFloatField;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        if (floatStatWithModifierInspectorAsset != null)
            root.Add(floatStatWithModifierInspectorAsset.CloneTree());

        baseValueFloatField = root.Q<FloatField>("inspector__base-value-float-field");

        SerializedProperty baseValueProperty = property.FindPropertyRelative("baseValue");
        baseValueFloatField.BindProperty(baseValueProperty);

        baseValueFloatField.RegisterValueChangedCallback(
            (evt) =>
            {
                property.GetValue<FloatStatWithModifier>().RecalculateFinalValue();
            }
        );

        return root;
    }
}

#endif
