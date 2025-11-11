#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class DescriptionEditor : EditorWindow
{
    public enum CurrentSOType
    {
        Skill,
        Item,
    }

    public CurrentSOType currentSOType = CurrentSOType.Skill;
    public VisualTreeAsset vTA;
    ObjectField scriptableObjectObjectField;
    TextField descTextField;
    Label descRenderedLabel;
    DropdownField addStylizedTextDropdownField;
    Button saveButton;
    Dictionary<string, string> stylizedTextMapper = new()
    {
        { "Passive", "<color=#3498DB>p</color>" },
        { "Active", "<color=#E67E22>a</color>" },
        { "HP", "<link=hp><color=#C71F37>hp</color></link>" },
        { "Current MP", "<link=current mp><color=#3E8EDE>current mp</color></link>" },
        { "Might", "<link=might><color=#F39C12>might</color></link>" },
        { "Reflex", "<link=reflex><color=#27AE60>reflex</color></link>" },
        { "Wisdom", "<link=wisdom><color=#9B59B6>wisdom</color></link>" },
        { "ASPD", "<link=aspd><color=#E67E22>aspd</color></link>" },
        { "Omnivamp", "<link=omnivamp><color=#C62828>omnivamp</color></link>" },
        { "ATK", "<link=atk><color=#E53935>atk</color></link>" },
        { "Armor", "<link=armor><color=#95A5A6>armor</color></link>" },
        {
            "Damage Reduction",
            "<link=damage reduction><color=#4A90E2>damage reduction</color></link>"
        },
        { "Positive Number", "<color=green>c</color>" },
        { "Strike Lock", "<link=strike lock><color=#fc03d7>strike lock</color></link>" },
    };
    int lastCaretIndex = 0;

    [MenuItem("Tools/DescriptionEditor")]
    private static void ShowWindow()
    {
        var window = GetWindow<DescriptionEditor>();
        window.titleContent = new GUIContent("DescriptionEditor");
        window.Show();
    }

    void CreateGUI()
    {
        vTA = Resources.Load<VisualTreeAsset>("VisualTreeAsset/DescriptionEditor");
        GetVisualElements();
        PopulateDropdown();
        AddEvents();
    }

    private void PopulateDropdown()
    {
        addStylizedTextDropdownField.choices = new()
        {
            "Passive",
            "Active",
            "HP",
            "Current MP",
            "Might",
            "Reflex",
            "Wisdom",
            "ASPD",
            "Omnivamp",
            "ATK",
            "Armor",
            "Damage Reduction",
            "Positive Number",
            "Strike Lock",
        };
    }

    private void AddEvents()
    {
        scriptableObjectObjectField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue is SkillDataSO)
            {
                descTextField.value = (evt.newValue as SkillDataSO).skillDescription;
                currentSOType = CurrentSOType.Skill;
            }
            else if (evt.newValue is ItemDataSO)
            {
                descTextField.value = (evt.newValue as ItemDataSO).itemDescription;
                currentSOType = CurrentSOType.Item;
            }
        });

        descTextField.RegisterValueChangedCallback(evt =>
        {
            descRenderedLabel.text = evt.newValue;
        });

        descTextField.RegisterCallback<FocusOutEvent>(evt =>
        {
            lastCaretIndex = descTextField.cursorIndex;
        });
        descTextField.RegisterCallback<FocusEvent>(evt =>
            descTextField.SelectRange(lastCaretIndex, lastCaretIndex)
        );

        addStylizedTextDropdownField.RegisterValueChangedCallback(evt =>
        {
            // add mapped text at caret position of descTextField
            descTextField.value = descTextField.value.Insert(
                lastCaretIndex,
                stylizedTextMapper[evt.newValue]
            );

            addStylizedTextDropdownField.SetValueWithoutNotify(null);
        });

        saveButton.clicked += () =>
        {
            if (currentSOType == CurrentSOType.Skill)
            {
                (scriptableObjectObjectField.value as SkillDataSO).skillDescription =
                    descTextField.value;
                EditorUtility.SetDirty(scriptableObjectObjectField.value);
            }
            else if (currentSOType == CurrentSOType.Item)
            {
                (scriptableObjectObjectField.value as ItemDataSO).itemDescription =
                    descTextField.value;
                EditorUtility.SetDirty(scriptableObjectObjectField.value);
            }

            AssetDatabase.SaveAssets();
        };
    }

    void GetVisualElements()
    {
        vTA.CloneTree(rootVisualElement);
        scriptableObjectObjectField = rootVisualElement.Q<ObjectField>("so-of");
        descTextField = rootVisualElement.Q<TextField>("desc-tf");
        addStylizedTextDropdownField = rootVisualElement.Q<DropdownField>("add-stylized-text-df");
        descRenderedLabel = rootVisualElement.Q<Label>("desc-rendered-l");
        saveButton = rootVisualElement.Q<Button>("save-b");
    }
}
#endif
