using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
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
        baseValueFloatField.dataSource = property.GetUnderlyingValue();

        baseValueFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(FloatStatWithModifier.BaseValue)),
                bindingMode = BindingMode.TwoWay,
            }
        );
        return root;
    }
}
