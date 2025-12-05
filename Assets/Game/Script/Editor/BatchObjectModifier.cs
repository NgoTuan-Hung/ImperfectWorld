#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BatchObjectModifier : EditorWindow
{
    [MenuItem("Window/Batch Object Modifier")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<BatchObjectModifier>();
        wnd.titleContent = new GUIContent("Batch Object Modifier");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ScrollView root = new ScrollView();

        var inspectorListView = new InspectorElement();
        ListObjectSO listObjectSO = CreateInstance<ListObjectSO>();
        inspectorListView.Bind(new SerializedObject(listObjectSO));

        Button button = new();
        button.text = "Modify";
        button.clicked += () =>
        {
            listObjectSO.objects.ForEach(o => ReplaceStatUpgradeSOString(o));
            AssetDatabase.SaveAssets();
        };

        root.Add(inspectorListView);
        root.Add(button);
        rootVisualElement.Add(root);
    }

    Dictionary<string, string> keyMap = new()
    {
        { "current hp", "CURHP" },
        { "hp", "HP" },
        { "hp regen", "HPREGEN" },
        { "current mp", "CURMP" },
        { "mp", "MP" },
        { "mp regen", "MPREGEN" },
        { "might", "MIGHT" },
        { "reflex", "REFLEX" },
        { "wisdom", "WISDOM" },
        { "aspd", "ASPD" },
        { "armor", "ARMOR" },
        { "mspd", "MSPD" },
        { "dmgmod", "DMGMOD" },
        { "omnivamp", "OMNIVAMP" },
        { "atk", "ATK" },
        { "crit", "CRIT" },
        { "critmod", "CRITMOD" },
        { "damage reduction", "DMGREDUC" },
        { "atkrange", "ATKRANGE" },
        { "strike lock", "STRIKELOCK" },
    };

    string ReplaceKeywords(string input)
    {
        // Sort keys by length (longest first) to avoid substring collisions
        foreach (var key in SortedKeysByLengthDesc())
        {
            string pattern = $@"\b{Regex.Escape(key)}\b";
            input = Regex.Replace(input, pattern, keyMap[key], RegexOptions.IgnoreCase);
        }
        return input;
    }

    List<string> SortedKeysByLengthDesc()
    {
        var list = new List<string>(keyMap.Keys);
        list.Sort((a, b) => b.Length.CompareTo(a.Length));
        return list;
    }

    void ReplaceString(Object o)
    {
        var skillSO = o as SkillDataSO;
        skillSO.skillDescription = ReplaceKeywords(skillSO.skillDescription);
        EditorUtility.SetDirty(skillSO);
    }

    void ReplaceItemSOString(Object o)
    {
        var itemSO = o as ItemDataSO;
        itemSO.itemDescription = ReplaceKeywords(itemSO.itemDescription);
        EditorUtility.SetDirty(itemSO);
    }

    void ReplaceStatUpgradeSOString(Object o)
    {
        var statUpgradeSO = o as StatUpgrade;
        statUpgradeSO.description = ReplaceKeywords(statUpgradeSO.description);
        EditorUtility.SetDirty(statUpgradeSO);
    }
}
#endif
