#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FindOffset : EditorWindow
{
    [MenuItem("Window/Find Offset")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<FindOffset>();
        wnd.titleContent = new GUIContent("Find Offset");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ObjectField gameObject1OF = new("GameObject 1"),
            gameObject2OF = new("GameObject 2");
        Label offsetLabel = new("Offset: ");
        Button findOffsetButton = new();
        findOffsetButton.text = "Find Offset";
        findOffsetButton.clicked += () =>
        {
            Vector3 offset =
                (gameObject1OF.value as GameObject).transform.position
                - (gameObject2OF.value as GameObject).transform.position;
            offsetLabel.text = "Offset: " + offset;
        };

        rootVisualElement.Add(gameObject1OF);
        rootVisualElement.Add(gameObject2OF);
        rootVisualElement.Add(offsetLabel);
        rootVisualElement.Add(findOffsetButton);
    }
}
#endif
