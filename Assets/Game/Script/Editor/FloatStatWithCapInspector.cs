using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FloatStatWithCap))]
public class FloatStatWithCapInspector : PropertyDrawer
{
    [SerializeField]
    private VisualTreeAsset floatStatWithCapInspectorAsset;
    FloatField floatField;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        if (floatStatWithCapInspectorAsset != null)
            root.Add(floatStatWithCapInspectorAsset.CloneTree());

        floatField = root.Q<FloatField>("inspector__float-field");

        floatField.dataSource = property.GetUnderlyingValue();
        floatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(FloatStatWithCap.Value)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        return root;
    }
}
