#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleRoomPopulator : EditorWindow
{
    public VisualTreeAsset vTA;
    IntegerField floorIntegerField;
    Button addButton;
    ListGameObjectSO listGameObjectSO;

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
        GetVisualElements();
        AddHandler();
    }

    private void AddHandler()
    {
        addButton.clicked += () => AddRoom(listGameObjectSO.gameObjects);
    }

    private void GetVisualElements()
    {
        var inspectorListView = new InspectorElement();
        listGameObjectSO = CreateInstance<ListGameObjectSO>();
        inspectorListView.Bind(new SerializedObject(listGameObjectSO));
        rootVisualElement.Add(inspectorListView);

        vTA.CloneTree(rootVisualElement);
        floorIntegerField = rootVisualElement.Q<IntegerField>("floor-if");
        addButton = rootVisualElement.Q<Button>("add-b");
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
        gameManager.AddNormalEnemyRoom(normalEnemyRoomInfo, floorIntegerField.value);
    }
}
#endif
