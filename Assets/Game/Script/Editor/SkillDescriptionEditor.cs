#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;

public class SkillDescriptionEditor : EditorWindow
{
    public VisualTreeAsset vTA;
    ObjectField skillSOObjectField;
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
        { "Positive Number", "<color=green>c</color>" },
        { "Strike Lock", "<link=strike lock><color=#fc03d7>strike lock</color></link>" },
    };
    int lastCaretIndex = 0;

    [MenuItem("Tools/SkillDescriptionEditor")]
    private static void ShowWindow()
    {
        var window = GetWindow<SkillDescriptionEditor>();
        window.titleContent = new GUIContent("SkillDescriptionEditor");
        window.Show();
    }

    void CreateGUI()
    {
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
            "Positive Number",
            "Strike Lock",
        };
    }

    private void AddEvents()
    {
        skillSOObjectField.RegisterValueChangedCallback(evt =>
        {
            var skillSO = evt.newValue as SkillDataSO;
            descTextField.value = skillSO.skillDescription;
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
        });

        saveButton.clicked += () =>
        {
            var skillSO = skillSOObjectField.value as SkillDataSO;
            skillSO.skillDescription = descTextField.value;
            EditorUtility.SetDirty(skillSO);
            AssetDatabase.SaveAssets();
        };
    }

    void GetVisualElements()
    {
        vTA.CloneTree(rootVisualElement);
        skillSOObjectField = rootVisualElement.Q<ObjectField>("skill-so-of");
        descTextField = rootVisualElement.Q<TextField>("desc-tf");
        addStylizedTextDropdownField = rootVisualElement.Q<DropdownField>("add-stylized-text-df");
        descRenderedLabel = rootVisualElement.Q<Label>("desc-rendered-l");
        saveButton = rootVisualElement.Q<Button>("save-b");
    }
}
#endif
