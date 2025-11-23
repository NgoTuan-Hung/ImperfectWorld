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
        { GetIDAsText(ComplexTextID.PASSIVE), GetColoredText(ComplexTextID.PASSIVE, "p") },
        { GetIDAsText(ComplexTextID.ACTIVE), GetColoredText(ComplexTextID.ACTIVE, "a") },
        { GetIDAsText(ComplexTextID.CURHP), GetMapper(ComplexTextID.CURHP) },
        { GetIDAsText(ComplexTextID.HP), GetMapper(ComplexTextID.HP) },
        { GetIDAsText(ComplexTextID.HPREGEN), GetMapper(ComplexTextID.HPREGEN) },
        { GetIDAsText(ComplexTextID.CURMP), GetMapper(ComplexTextID.CURMP) },
        { GetIDAsText(ComplexTextID.MP), GetMapper(ComplexTextID.MP) },
        { GetIDAsText(ComplexTextID.MPREGEN), GetMapper(ComplexTextID.MPREGEN) },
        { GetIDAsText(ComplexTextID.MIGHT), GetMapper(ComplexTextID.MIGHT) },
        { GetIDAsText(ComplexTextID.REFLEX), GetMapper(ComplexTextID.REFLEX) },
        { GetIDAsText(ComplexTextID.WISDOM), GetMapper(ComplexTextID.WISDOM) },
        { GetIDAsText(ComplexTextID.ASPD), GetMapper(ComplexTextID.ASPD) },
        { GetIDAsText(ComplexTextID.ARMOR), GetMapper(ComplexTextID.ARMOR) },
        { GetIDAsText(ComplexTextID.MSPD), GetMapper(ComplexTextID.MSPD) },
        { GetIDAsText(ComplexTextID.DMGMOD), GetMapper(ComplexTextID.DMGMOD) },
        { GetIDAsText(ComplexTextID.OMNIVAMP), GetMapper(ComplexTextID.OMNIVAMP) },
        { GetIDAsText(ComplexTextID.ATK), GetMapper(ComplexTextID.ATK) },
        { GetIDAsText(ComplexTextID.CRIT), GetMapper(ComplexTextID.CRIT) },
        { GetIDAsText(ComplexTextID.CRITMOD), GetMapper(ComplexTextID.CRITMOD) },
        { GetIDAsText(ComplexTextID.DMGREDUC), GetMapper(ComplexTextID.DMGREDUC) },
        { GetIDAsText(ComplexTextID.ATKRANGE), GetMapper(ComplexTextID.ATKRANGE) },
        {
            GetIDAsText(ComplexTextID.POSITIVENUMBER),
            GetColoredText(ComplexTextID.POSITIVENUMBER, "0")
        },
        { GetIDAsText(ComplexTextID.STRIKELOCK), GetMapper(ComplexTextID.STRIKELOCK) },
    };
    int lastCaretIndex = 0;

    static string GetIDAsText(ComplexTextID id) => GameManager.GetComplexTextIDAsText(id);

    static string GetMapper(ComplexTextID id) => GameManager.ConstructComplexText(id);

    static string GetColoredText(ComplexTextID id, string innerText) =>
        GameManager.ConstructColoredText(id, innerText);

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
        addStylizedTextDropdownField.choices = GameManager.GetAllIDsAsText();
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
