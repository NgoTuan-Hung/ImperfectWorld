#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BatchStatModifier : EditorWindow
{
    [MenuItem("Window/Batch Stat Modifier")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<BatchStatModifier>();
        wnd.titleContent = new GUIContent("Batch Stat Modifier");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        var inspectorListView = new InspectorElement();
        ListStatSO listStatSO = CreateInstance<ListStatSO>();
        inspectorListView.Bind(new SerializedObject(listStatSO));

        Toggle critChanceToggle = new();
        critChanceToggle.label = "Critical Chance";
        critChanceToggle.value = false;

        FloatField critChanceFloatField = new();
        critChanceFloatField.label = "Critical Chance";
        critChanceFloatField.value = 0f;

        Toggle critDamageModifierToggle = new();
        critDamageModifierToggle.label = "Critical Damage Modifier";
        critDamageModifierToggle.value = false;

        FloatField critDamageModifierFloatField = new();
        critDamageModifierFloatField.label = "Critical Damage Modifier";
        critDamageModifierFloatField.value = 0f;

        Button button = new();
        button.text = "Modify Game Effect SO";
        button.clicked += () =>
        {
            listStatSO.stats.ForEach(stat =>
            {
                if (critChanceToggle.value)
                    stat.critChance.BaseValue = critChanceFloatField.value;
                if (critDamageModifierToggle.value)
                    stat.critDamageModifier.BaseValue = critDamageModifierFloatField.value;
            });
        };

        rootVisualElement.Add(inspectorListView);
        rootVisualElement.Add(critChanceToggle);
        rootVisualElement.Add(critChanceFloatField);
        rootVisualElement.Add(critDamageModifierToggle);
        rootVisualElement.Add(critDamageModifierFloatField);
        rootVisualElement.Add(button);
    }
}
#endif
