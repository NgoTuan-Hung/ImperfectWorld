#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Just automation script, nothing special
/// </summary>
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
        var inspectorListView = new InspectorElement();
        ListGameObject prefabs = CreateInstance<ListGameObject>();
        inspectorListView.Bind(new SerializedObject(prefabs));
        rootVisualElement.Add(inspectorListView);

        Button button = new();
        button.text = "Modify Game Effect Prefab";
        button.clicked += () =>
        {
            // Modify(((GameObject)objectField.value));
            // prefabs.gameObjects.ForEach(gO => Modify(gO));
            // prefabs.gameObjects.ForEach(gO => AddMissingAnimator(gO));
            // prefabs.gameObjects.ForEach(gO => ChangeCollider(gO));
            // prefabs.gameObjects.ForEach(gO => ResetGameEffectPrefab(gO));
            // prefabs.gameObjects.ForEach(gO => TurnOnTrigger(gO));
            // prefabs.gameObjects.ForEach(gO => ChangeCollidersLayer(gO));
            prefabs.gameObjects.ForEach(gO => RemoveAnimatorFromColliders(gO));
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

        SpriteRenderer spriteRenderer = p_gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.gameObject.name = "AnimateObject";
        spriteRenderer.transform.SetParent(animateObjects.transform, true);
        spriteRenderer.gameObject.AddComponent<AnimateObject>();

        if (spriteRenderer.transform.childCount > 0)
        {
            GameObject secondarySpriteRenderer = spriteRenderer.transform.GetChild(0).gameObject;
            secondarySpriteRenderer.name = "AnimateObject1";
            secondarySpriteRenderer.transform.SetParent(animateObjects.transform, true);
            secondarySpriteRenderer.AddComponent<Animator>();
            secondarySpriteRenderer.AddComponent<AnimateObject>();
        }

        GameEffectPrefab gameEffectPrefab = p_gameObject.GetComponent<GameEffectPrefab>();
        gameEffectPrefab.Reset();
    }

    void AddMissingAnimator(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;

        SpriteRenderer spriteRenderer = p_gameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer.GetComponent<Animator>() == null)
        {
            spriteRenderer.gameObject.AddComponent<Animator>();
            spriteRenderer.gameObject.GetComponent<AnimateObject>().Reset();
        }
    }

    void ChangeCollider(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;

        GameObject colliders = new("Colliders");
        colliders.transform.SetParent(p_gameObject.transform);
        colliders.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        BoxCollider2D currentBC = p_gameObject.GetComponent<BoxCollider2D>();
        if (currentBC != null)
        {
            GameObject newBCGO = new("BoxCollider2D");
            newBCGO.transform.SetParent(colliders.transform);
            newBCGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            BoxCollider2D newBC = newBCGO.AddComponent<BoxCollider2D>();
            newBC.isTrigger = true;
            newBC.offset = currentBC.offset;
            newBC.size = currentBC.size;

            DestroyImmediate(currentBC);
        }

        PolygonCollider2D currentPC = p_gameObject.GetComponent<PolygonCollider2D>();
        if (currentPC != null)
        {
            GameObject newPCGO = new("PolygonCollider2D");
            newPCGO.transform.SetParent(colliders.transform);
            newPCGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            PolygonCollider2D newBC = newPCGO.AddComponent<PolygonCollider2D>();
            newBC.isTrigger = true;
            newBC.offset = currentPC.offset;
            newBC.points = currentPC.points;

            DestroyImmediate(currentPC);
        }

        CircleCollider2D currentCC = p_gameObject.GetComponent<CircleCollider2D>();
        if (currentCC != null)
        {
            GameObject newCCGO = new("CircleCollider2D");
            newCCGO.transform.SetParent(colliders.transform);
            newCCGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            CircleCollider2D newCC = newCCGO.AddComponent<CircleCollider2D>();
            newCC.isTrigger = true;
            newCC.offset = currentCC.offset;
            newCC.radius = currentCC.radius;

            DestroyImmediate(currentCC);
        }
    }

    void ResetGameEffectPrefab(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;
        p_gameObject.GetComponent<GameEffectPrefab>()?.Reset();
    }

    void TurnOnTrigger(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;

        List<Collider2D> collider2Ds = p_gameObject.GetComponentsInChildren<Collider2D>().ToList();
        foreach (Collider2D collider2D in collider2Ds)
        {
            collider2D.isTrigger = true;
        }
    }

    void ChangeCollidersLayer(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;

        List<Collider2D> collider2Ds = p_gameObject.GetComponentsInChildren<Collider2D>().ToList();
        foreach (Collider2D collider2D in collider2Ds)
        {
            collider2D.gameObject.layer = LayerMask.NameToLayer("CombatCollider");
        }
    }

    void RemoveAnimatorFromColliders(GameObject p_gameObject)
    {
        using var editingScope = new PrefabUtility.EditPrefabContentsScope(
            AssetDatabase.GetAssetPath(p_gameObject)
        );
        p_gameObject = editingScope.prefabContentsRoot;

        List<Collider2D> collider2Ds = p_gameObject.GetComponentsInChildren<Collider2D>().ToList();
        foreach (Collider2D collider2D in collider2Ds)
        {
            var animator = collider2D.gameObject.GetComponent<Animator>();
            if (animator != null)
                DestroyImmediate(animator);
        }
    }
}
#endif
