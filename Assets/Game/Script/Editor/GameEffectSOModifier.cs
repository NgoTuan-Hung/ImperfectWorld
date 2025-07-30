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
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameEffectSO.gameEffectPrefab.gameObject)
        );
        GameObject t_gameObject = editingScope.prefabContentsRoot;

        GameObject.DestroyImmediate(t_gameObject.GetComponent<GameEffectPrefab>());

        GameEffect t_gameEffect = t_gameObject.AddComponent<GameEffect>();
        t_gameEffect.gameEffectSO = p_gameEffectSO;

        p_gameEffectSO.gameEffectBehaviours.ForEach(gEB =>
        {
            switch (gEB)
            {
                case EGameEffectBehaviour.CollideAndDamage:
                    t_gameObject.AddComponent<CollideAndDamage>();
                    break;
                case EGameEffectBehaviour.BlueHole:
                    t_gameObject.AddComponent<BlueHole>();
                    break;
                case EGameEffectBehaviour.InfernalTideFanReceiver:
                    t_gameObject.AddComponent<InfernalTideFanReceiver>();
                    break;
                default:
                    break;
            }
        });

        Rigidbody2D t_rigidBody2D;
        if ((t_rigidBody2D = t_gameObject.GetComponent<Rigidbody2D>()) == null)
            t_rigidBody2D = t_gameObject.AddComponent<Rigidbody2D>();
        t_rigidBody2D.excludeLayers = p_gameEffectSO.collisionExcludeLayerMask;
        t_rigidBody2D.gravityScale = 0;

        t_gameObject.layer = LayerMask.NameToLayer("CombatCollider");
        t_gameObject.tag = p_gameEffectSO.tag;
    }
}
