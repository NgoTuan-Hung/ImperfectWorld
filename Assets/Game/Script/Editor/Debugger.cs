#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class Debugger : EditorWindow
{
    public VisualTreeAsset vTA;
    ObjectField gameObjectField;
    TextField componentNameTextField;
    Button getAllComponentFieldButton;
    Label componentFieldsLabel;

    [MenuItem("Tools/Debugger")]
    private static void ShowWindow()
    {
        var window = GetWindow<Debugger>();
        window.titleContent = new GUIContent("Debugger");
        window.Show();
    }

    void CreateGUI()
    {
        vTA.CloneTree(rootVisualElement);
        gameObjectField = rootVisualElement.Q<ObjectField>("game-object-of");
        componentNameTextField = rootVisualElement.Q<TextField>("component-name-tf");
        getAllComponentFieldButton = rootVisualElement.Q<Button>("get-all-component-fields-b");
        componentFieldsLabel = rootVisualElement.Q<Label>("component-fields-l");

        getAllComponentFieldButton.clicked += GetAllComponentField;
    }

    void GetAllComponentField()
    {
        var componentName = componentNameTextField.text;
        if (string.IsNullOrEmpty(componentName))
        {
            componentFieldsLabel.text = "Please input component name first!";
            return;
        }

        var component = (gameObjectField.value as GameObject).GetComponent(componentName);

        var fields = component.GetType().GetFields();
        var sb = new System.Text.StringBuilder();
        foreach (var field in fields)
        {
            sb.AppendLine(field.Name + " - " + (field.GetValue(component) ?? "NULL"));
        }
        componentFieldsLabel.text = sb.ToString();
    }
}
#endif
