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
    DropdownField roomTypeDF;

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
        roomTypeDF = rootVisualElement.Q<DropdownField>("room-type-df");
        addButton = rootVisualElement.Q<Button>("add-b");
    }

    void AddRoom(List<GameObject> gameObjects)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        EnemyRoomInfo enemyRoomInfo = new();
        gameObjects.ForEach(gO =>
            enemyRoomInfo.roomEnemyInfos.Add(
                new(PrefabUtility.GetCorrespondingObjectFromSource(gO), gO.transform.position)
            )
        );

        switch (roomTypeDF.value)
        {
            case "Normal":
            {
                gameManager.AddNormalEnemyRoom(enemyRoomInfo, floorIntegerField.value);
                break;
            }
            case "Elite":
            {
                gameManager.AddEliteRoom(enemyRoomInfo);
                break;
            }
            case "Boss":
            {
                gameManager.AddBossRoom(enemyRoomInfo);
                break;
            }
            default:
                break;
        }
    }
}
#endif
