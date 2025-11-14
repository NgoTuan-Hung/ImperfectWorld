#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BattleRoomPopulator : EditorWindow
{
    [MenuItem("Window/Battle Room Populator")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<BattleRoomPopulator>();
        wnd.titleContent = new GUIContent("Battle Room Populator");

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
        button.text = "ADD ROOM";
        button.clicked += () => AddRoom(listGameObjectSO.gameObjects);

        root.Add(inspectorListView);
        root.Add(button);
        rootVisualElement.Add(root);
    }

    void AddRoom(List<GameObject> gameObjects)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        NormalEnemyRoomInfo normalEnemyRoomInfo = new();
        gameObjects.ForEach(gO =>
            normalEnemyRoomInfo.roomEnemyInfos.Add(
                new(PrefabUtility.GetCorrespondingObjectFromSource(gO), gO.transform.position)
            )
        );
        gameManager.AddNormalEnemyRoom(normalEnemyRoomInfo);
    }
}
#endif
