#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
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

        SerializedProperty baseValueProperty = property.FindPropertyRelative("value");
        floatField.BindProperty(baseValueProperty);

        floatField.RegisterValueChangedCallback(
            (evt) =>
            {
                property.GetValue<FloatStatWithCap>().valueChangeEvent();
            }
        );
        return root;
    }
}
#endif
