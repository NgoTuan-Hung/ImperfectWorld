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
        StatUpgrade,
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
        { "PASSIVE", "<color=#3498DB>p</color>" },
        { "ACTIVE", "<color=#E67E22>a</color>" },
        { "CURRENT HP", "<link=current hp><color=#D72638>current hp</color></link>" },
        { "HP", "<link=hp><color=#C71F37>hp</color></link>" },
        { "HPREGEN", "<link=hp regen><color=#FF6B6B>hp regen</color></link>" },
        { "CURRENT MP", "<link=current mp><color=#3E8EDE>current mp</color></link>" },
        { "MP", "<link=mp><color=#2F75C0>mp</color></link>" },
        { "MPREGEN", "<link=mp regen><color=#7FDBFF>mp regen</color></link>" },
        { "MIGHT", "<link=might><color=#F39C12>might</color></link>" },
        { "REFLEX", "<link=reflex><color=#27AE60>reflex</color></link>" },
        { "WISDOM", "<link=wisdom><color=#9B59B6>wisdom</color></link>" },
        { "ASPD", "<link=aspd><color=#E67E22>aspd</color></link>" },
        { "ARMOR", "<link=armor><color=#95A5A6>armor</color></link>" },
        { "MSPD", "<link=mspd><color=#1ABC9C>mspd</color></link>" },
        { "DMGMOD", "<link=dmgmod><color=#FFC107>dmgmod</color></link>: damage multiplier." },
        { "OMNIVAMP", "<link=omnivamp><color=#C62828>omnivamp</color></link>" },
        { "ATK", "<link=atk><color=#E53935>atk</color></link>" },
        { "CRIT", "<link=crit><color=#FFD54F>crit</color></link>: crit chance." },
        {
            "CRITMOD",
            "<link=critmod><color=#AB47BC>critmod</color></link>: crit damage modifier, how much damage is multiplied on crit."
        },
        { "DMGREDUC", "<link=damage reduction><color=#4A90E2>damage reduction</color></link>" },
        { "ATKRANGE", "<link=atkrange><color=#FFD447>atkrange</color></link>: attack range." },
        { "POSITIVE NUMBER", "<color=green>c</color>" },
        { "STRIKE LOCK", "<link=strike lock><color=#fc03d7>strike lock</color></link>" },
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
            "PASSIVE",
            "ACTIVE",
            "CURRENT HP",
            "HP",
            "HPREGEN",
            "CURRENT MP",
            "MP",
            "MPREGEN",
            "MIGHT",
            "REFLEX",
            "WISDOM",
            "ASPD",
            "ARMOR",
            "MSPD",
            "DMGMOD",
            "OMNIVAMP",
            "ATK",
            "CRIT",
            "CRITMOD",
            "DMGREDUC",
            "ATKRANGE",
            "POSITIVE NUMBER",
            "STRIKE LOCK",
        };
    }

    private void AddEvents()
    {
        scriptableObjectObjectField.RegisterValueChangedCallback(evt =>
        {
            switch (evt.newValue)
            {
                case SkillDataSO skillData:
                    descTextField.value = skillData.skillDescription;
                    currentSOType = CurrentSOType.Skill;
                    break;

                case ItemDataSO itemData:
                    descTextField.value = itemData.itemDescription;
                    currentSOType = CurrentSOType.Item;
                    break;

                case StatUpgrade statUpgradeData:
                    descTextField.value = statUpgradeData.description;
                    currentSOType = CurrentSOType.StatUpgrade;
                    break;

                default:
                    break;
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
            switch (currentSOType)
            {
                case CurrentSOType.Skill:
                {
                    (scriptableObjectObjectField.value as SkillDataSO).skillDescription =
                        descTextField.value;
                    break;
                }
                case CurrentSOType.Item:
                {
                    (scriptableObjectObjectField.value as ItemDataSO).itemDescription =
                        descTextField.value;
                    break;
                }
                case CurrentSOType.StatUpgrade:
                {
                    (scriptableObjectObjectField.value as StatUpgrade).description =
                        descTextField.value;
                    break;
                }
                default:
                    break;
            }

            EditorUtility.SetDirty(scriptableObjectObjectField.value);

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
