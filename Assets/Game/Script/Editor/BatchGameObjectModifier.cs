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
                BatchRename(gO);
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

    void StoreStatToDisk(GameObject gO)
    {
        Stat stat = gO.GetComponent<Stat>();
        ChampionData championData = gO.GetComponent<CustomMono>().championData;
        championData.healthPoint = stat.healthPoint.BaseValue;
        championData.healthRegen = stat.healthRegen.BaseValue;
        championData.manaPoint = stat.manaPoint.BaseValue;
        championData.manaRegen = stat.manaRegen.BaseValue;
        championData.might = stat.might.BaseValue;
        championData.reflex = stat.reflex.BaseValue;
        championData.wisdom = stat.wisdom.BaseValue;
        championData.attackSpeed = stat.attackSpeed.BaseValue;
        championData.armor = stat.armor.BaseValue;
        championData.moveSpeed = stat.moveSpeed.BaseValue;
        championData.damageModifier = stat.damageModifier.BaseValue;
        championData.omnivamp = stat.omnivamp.BaseValue;
        championData.attackDamage = stat.attackDamage.BaseValue;
        championData.critChance = stat.critChance.BaseValue;
        championData.critDamageModifier = stat.critDamageModifier.BaseValue;
        championData.damageReduction = stat.damageReduction.BaseValue;
        championData.attackRange = stat.attackRange.BaseValue;
        EditorUtility.SetDirty(championData);
    }

    void BatchRename(GameObject gO)
    {
        ChampionData championData = gO.GetComponent<CustomMono>().championData;
        string assetPath = AssetDatabase.GetAssetPath(championData.GetInstanceID());
        string result = AssetDatabase.RenameAsset(
            assetPath,
            championData.name.Replace("CAI", "CD") + ".asset"
        );
        Debug.Log(result);
    }
}
#endif
