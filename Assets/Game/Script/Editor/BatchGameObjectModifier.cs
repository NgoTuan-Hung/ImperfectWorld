#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BatchGameObjectModifier : EditorWindow
{
    [MenuItem("Window/Batch Game Object Modifier")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<BatchGameObjectModifier>();
        wnd.titleContent = new GUIContent("Batch Game Object Modifier");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ScrollView root = new ScrollView();

        var inspectorListView = new InspectorElement();
        ListGameObjectSO listGameObjectSO = CreateInstance<ListGameObjectSO>();
        inspectorListView.Bind(new SerializedObject(listGameObjectSO));

        Button button = new();
        button.text = "Modify";
        button.clicked += () =>
        {
            listGameObjectSO.gameObjects.ForEach(gO =>
            {
                ChangeArrowLayer(gO);
            });
            AssetDatabase.SaveAssets();
        };

        root.Add(inspectorListView);
        root.Add(button);
        rootVisualElement.Add(root);
    }

    void DestroySRAndSG(GameObject gO)
    {
        var dI = gO.transform.Find("DirectionIndicator").gameObject;
        DestroyImmediate(dI.GetComponent<SpriteRenderer>(), true);
        DestroyImmediate(dI.GetComponent<SortingGroup>(), true);
        EditorUtility.SetDirty(gO);
    }

    void ChangeArrowLayer(GameObject gO)
    {
        var sprite = gO
            .transform.Find("DirectionIndicator/ArrowIndicator")
            .gameObject.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Base";
        sprite.sortingOrder = 0;
        EditorUtility.SetDirty(gO);
    }
}
#endif
