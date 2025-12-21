#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using OneLine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TemporaryTool : EditorWindow
{
    [MenuItem("Window/Temporary Tool")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<TemporaryTool>();
        wnd.titleContent = new GUIContent("Temporary Tool");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ScrollView root = new ScrollView();

        Button button = new();
        button.text = "Check Champion Distribution";
        button.clicked += CheckChampionDistribution;

        Button dialogButton = new();
        dialogButton.text = "Show Dialog";
        dialogButton.clicked += () => GameUIManager.Instance.ShowGuideDialogBox();

        root.Add(button);
        root.Add(dialogButton);
        rootVisualElement.Add(root);
    }

    void CheckChampionDistribution()
    {
        Dictionary<GameObject, int> championAndCount = new Dictionary<GameObject, int>();

        var champions = Resources.LoadAll<GameObject>("Champion/");
        champions.ForEach(champion => championAndCount.Add(champion, 0));

        var floors = FindFirstObjectByType<GameManager>().roomSystem.normalEnemyFloors;
        floors.ForEach(floor =>
        {
            floor.normalEnemyRoomInfos.ForEach(nERI =>
            {
                nERI.roomEnemyInfos.ForEach(rEI => championAndCount[rEI.prefab]++);
            });
        });

        StringBuilder result = new StringBuilder();
        championAndCount.ForEach(championAndCountElement =>
        {
            result.AppendLine(
                championAndCountElement.Key.name + " - " + championAndCountElement.Value
            );
        });
        Debug.Log(result);
    }
}
#endif
