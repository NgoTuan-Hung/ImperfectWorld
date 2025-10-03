#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(EnhancedOnScreenStick))]
public class EnhancedOnScreenStickEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        AddPropertyField(root, "stickType");
        AddPropertyField(root, "movementRange");
        AddPropertyField(root, "deadZone");
        AddPropertyField(root, "showOnlyWhenPressed");

        AddSpace(root, 6f);

        AddPropertyField(root, "background");
        AddPropertyField(root, "handle");

        return root;
    }

    void AddPropertyField(VisualElement root, string propertyName)
    {
        root.Add(new PropertyField(serializedObject.FindProperty(propertyName)));
    }

    void AddSpace(VisualElement root, float space)
    {
        root.Add(new VisualElement() { style = { height = space } });
    }
}

#endif
