using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEffectSOModifier : EditorWindow
{
    [MenuItem("Window/Modify Game Effect SO")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<GameEffectSOModifier>();
        wnd.titleContent = new GUIContent("Modify Game Effect SO");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        var inspectorListView = new InspectorElement();
        ListGameEffectSO gameEffectSOs = CreateInstance<ListGameEffectSO>();
        inspectorListView.Bind(new SerializedObject(gameEffectSOs));
        rootVisualElement.Add(inspectorListView);

        Button button = new();
        button.text = "Modify Game Effect SO";
        button.clicked += () =>
        {
            gameEffectSOs.gameEffectSOs.ForEach(gESO => ModifyBehavior(gESO));
        };
        rootVisualElement.Add(button);
    }

    void ModifyBehavior(GameEffectSO p_gameEffectSO)
    {
        p_gameEffectSO.gameEffectBehaviours.Add(EGameEffectBehaviour.CollideAndDamage);
    }
}
