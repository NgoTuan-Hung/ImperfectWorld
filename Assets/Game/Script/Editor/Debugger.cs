#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Reflection;

public class Debugger : EditorWindow
{
    public VisualTreeAsset vTA;
    ObjectField gameObjectField;
    TextField componentNameTextField,
        findFieldPathTextField,
        findFieldValueTextField;
    Button getAllComponentFieldButton,
        findFieldButton;
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
        findFieldButton = rootVisualElement.Q<Button>("find-field-b");
        findFieldPathTextField = rootVisualElement.Q<TextField>("find-field-path-tf");
        findFieldValueTextField = rootVisualElement.Q<TextField>("find-field-value-tf");

        getAllComponentFieldButton.clicked += GetAllComponentField;
        findFieldButton.clicked += FindField;
    }

    private void FindField()
    {
        PrintPathValue(gameObjectField.value as GameObject, findFieldPathTextField.text);
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

    public void PrintPathValue(GameObject root, string path)
    {
        if (root == null || string.IsNullOrEmpty(path))
        {
            Debug.LogError("Root or path is null");
            return;
        }

        object current = root;
        string[] parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            // ─────────────────────────────
            // CASE 1: Handle "..Component"
            // ─────────────────────────────
            if (part.StartsWith(".."))
            {
                // remove leading ..
                string compAndFields = part.Substring(2);

                // Separate component name from possible field chain
                string[] compSplit = compAndFields.Split(
                    '.',
                    2,
                    StringSplitOptions.RemoveEmptyEntries
                );
                string compName = compSplit[0];
                string fieldChain = compSplit.Length > 1 ? compSplit[1] : null;

                GameObject go = current as GameObject;
                if (go == null)
                {
                    Debug.LogError("Cannot get component from non-GameObject object.");
                    return;
                }

                Component comp = go.GetComponent(compName);
                if (comp == null)
                {
                    Debug.LogError($"Component '{compName}' not found on '{go.name}'.");
                    return;
                }

                current = comp;

                if (!string.IsNullOrEmpty(fieldChain))
                {
                    current = ResolveFieldChain(current, fieldChain);
                    if (current == null)
                        return;
                }
            }
            // ─────────────────────────────
            // CASE 2: Hierarchy traversal + optional ..Component access
            // ─────────────────────────────
            else if (!part.StartsWith("."))
            {
                if (part.Contains(".."))
                {
                    // Example: "Hand..Stat.Physique"
                    string[] split = part.Split("..", StringSplitOptions.RemoveEmptyEntries);
                    string childName = split.Length > 1 ? split[0] : "";
                    string compAndFields = split[^1];

                    string[] subParts = compAndFields.Split(
                        '.',
                        2,
                        StringSplitOptions.RemoveEmptyEntries
                    );
                    string compName = subParts[0];
                    string fieldChain = subParts.Length > 1 ? subParts[1] : null;

                    GameObject targetGO = current as GameObject;
                    if (!string.IsNullOrEmpty(childName))
                        targetGO = targetGO?.transform.Find(childName)?.gameObject;

                    if (targetGO == null)
                    {
                        Debug.LogError($"GameObject '{childName}' not found.");
                        return;
                    }

                    Component comp = targetGO.GetComponent(compName);
                    if (comp == null)
                    {
                        Debug.LogError($"Component '{compName}' not found on '{targetGO.name}'.");
                        return;
                    }

                    current = comp;

                    if (!string.IsNullOrEmpty(fieldChain))
                    {
                        current = ResolveFieldChain(current, fieldChain);
                        if (current == null)
                            return;
                    }
                }
                else
                {
                    // Just hierarchy navigation (Body / Hand / etc)
                    GameObject next = (current as GameObject)?.transform.Find(part)?.gameObject;
                    if (next == null)
                    {
                        Debug.LogError($"Child '{part}' not found.");
                        return;
                    }
                    current = next;
                }
            }
            // ─────────────────────────────
            // CASE 3: Pure field/property chain (.field1.field2)
            // ─────────────────────────────
            else
            {
                string fieldChain = part.TrimStart('.');
                current = ResolveFieldChain(current, fieldChain);
                if (current == null)
                    return;
            }
        }

        findFieldValueTextField.value = $"[PathValueReader] Value at '{path}' = {current}";
    }

    private object ResolveFieldChain(object obj, string chain)
    {
        string[] fields = chain.Split('.', StringSplitOptions.RemoveEmptyEntries);
        object current = obj;

        foreach (string fieldName in fields)
        {
            Type t = current.GetType();
            FieldInfo f = t.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );
            PropertyInfo p = t.GetProperty(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (f != null)
                current = f.GetValue(current);
            else if (p != null)
                current = p.GetValue(current);
            else
            {
                Debug.LogError($"Field/Property '{fieldName}' not found in {t.Name}");
                return null;
            }
        }
        return current;
    }
}
#endif
