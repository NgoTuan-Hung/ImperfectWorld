using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEffectPrefabModifier : EditorWindow
{
    [MenuItem("Window/Modify Game Effect Prefab")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<GameEffectPrefabModifier>();
        wnd.titleContent = new GUIContent("Modify Game Effect Prefab");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ObjectField objectField = new();
        rootVisualElement.Add(objectField);

        var inspectorListView = new InspectorElement();
        ListGameObject prefabs = new ListGameObject();
        inspectorListView.Bind(new SerializedObject(prefabs));
        rootVisualElement.Add(inspectorListView);

        Button button = new();
        button.text = "Modify Game Effect Prefab";
        button.clicked += () =>
        {
            // Modify(((GameObject)objectField.value));
            prefabs.gameObjects.ForEach(gO => Debug.Log(gO.name));
        };
        rootVisualElement.Add(button);
    }

    void Modify(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;
        GameObject animateObjects = new GameObject("AnimateObjects");

        animateObjects.transform.SetParent(p_gameObject.transform);
        animateObjects.transform.localPosition = Vector3.zero;
        animateObjects.transform.localRotation = Quaternion.identity;
        animateObjects.transform.localScale = Vector3.one;

        GameEffectPrefab gameEffectPrefab = p_gameObject.GetComponent<GameEffectPrefab>();
        SpriteRenderer spriteRenderer = p_gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.gameObject.name = "AnimateObject";
        spriteRenderer.transform.SetParent(animateObjects.transform, true);
        spriteRenderer.gameObject.AddComponent<AnimateObject>();

        gameEffectPrefab.Reset();
    }
}
