#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BatchStatModifier : EditorWindow
{
    [MenuItem("Window/Batch Stat Modifier")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<BatchStatModifier>();
        wnd.titleContent = new GUIContent("Batch Stat Modifier");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ScrollView root = new ScrollView();

        var inspectorListView = new InspectorElement();
        ListStatSO listStatSO = CreateInstance<ListStatSO>();
        inspectorListView.Bind(new SerializedObject(listStatSO));

        Toggle hpToggle = new();
        hpToggle.label = "HP";
        hpToggle.value = false;

        FloatField hpFloatField = new();
        hpFloatField.label = "HP";
        hpFloatField.value = 0f;

        Toggle mightToggle = new();
        mightToggle.label = "Might";
        mightToggle.value = false;

        FloatField mightFloatField = new();
        mightFloatField.label = "Might";
        mightFloatField.value = 0f;

        Toggle reflexToggle = new();
        reflexToggle.label = "Reflex";
        reflexToggle.value = false;

        FloatField reflexFloatField = new();
        reflexFloatField.label = "Reflex";
        reflexFloatField.value = 0f;

        Toggle wisdomToggle = new();
        wisdomToggle.label = "Wisdom";
        wisdomToggle.value = false;

        FloatField wisdomFloatField = new();
        wisdomFloatField.label = "Wisdom";
        wisdomFloatField.value = 0f;

        Toggle attackSpeedToggle = new();
        attackSpeedToggle.label = "Attack Speed";
        attackSpeedToggle.value = false;

        FloatField attackSpeedFloatField = new();
        attackSpeedFloatField.label = "Attack Speed";
        attackSpeedFloatField.value = 0f;

        Toggle armorToggle = new();
        armorToggle.label = "Armor";
        armorToggle.value = false;

        FloatField armorFloatField = new();
        armorFloatField.label = "Armor";
        armorFloatField.value = 0f;

        Toggle moveSpeedToggle = new();
        moveSpeedToggle.label = "Move Speed";
        moveSpeedToggle.value = false;

        FloatField moveSpeedFloatField = new();
        moveSpeedFloatField.label = "Move Speed";
        moveSpeedFloatField.value = 0f;

        Toggle attackDamageToggle = new();
        attackDamageToggle.label = "Attack Damage";
        attackDamageToggle.value = false;

        FloatField attackDamageFloatField = new();
        attackDamageFloatField.label = "Attack Damage";
        attackDamageFloatField.value = 0f;

        Toggle critChanceToggle = new();
        critChanceToggle.label = "Critical Chance";
        critChanceToggle.value = false;

        FloatField critChanceFloatField = new();
        critChanceFloatField.label = "Critical Chance";
        critChanceFloatField.value = 0f;

        Toggle critDamageModifierToggle = new();
        critDamageModifierToggle.label = "Critical Damage Modifier";
        critDamageModifierToggle.value = false;

        FloatField critDamageModifierFloatField = new();
        critDamageModifierFloatField.label = "Critical Damage Modifier";
        critDamageModifierFloatField.value = 0f;

        Button button = new();
        button.text = "Modify";
        button.clicked += () =>
        {
            listStatSO.stats.ForEach(stat =>
            {
                if (hpToggle.value)
                    stat.healthPoint.BaseValue = hpFloatField.value;
                if (mightToggle.value)
                    stat.might.BaseValue = mightFloatField.value;
                if (reflexToggle.value)
                    stat.reflex.BaseValue = reflexFloatField.value;
                if (wisdomToggle.value)
                    stat.wisdom.BaseValue = wisdomFloatField.value;
                if (attackSpeedToggle.value)
                    stat.attackSpeed.BaseValue = attackSpeedFloatField.value;
                if (armorToggle.value)
                    stat.armor.BaseValue = armorFloatField.value;
                if (moveSpeedToggle.value)
                    stat.moveSpeed.BaseValue = moveSpeedFloatField.value;
                if (attackDamageToggle.value)
                    stat.attackDamage.BaseValue = attackDamageFloatField.value;
                if (critChanceToggle.value)
                    stat.critChance.BaseValue = critChanceFloatField.value;
                if (critDamageModifierToggle.value)
                    stat.critDamageModifier.BaseValue = critDamageModifierFloatField.value;
                EditorUtility.SetDirty(stat);
            });
            AssetDatabase.SaveAssets();
        };

        root.Add(inspectorListView);
        root.Add(hpToggle);
        root.Add(hpFloatField);
        root.Add(mightToggle);
        root.Add(mightFloatField);
        root.Add(reflexToggle);
        root.Add(reflexFloatField);
        root.Add(wisdomToggle);
        root.Add(wisdomFloatField);
        root.Add(attackSpeedToggle);
        root.Add(attackSpeedFloatField);
        root.Add(armorToggle);
        root.Add(armorFloatField);
        root.Add(moveSpeedToggle);
        root.Add(moveSpeedFloatField);
        root.Add(attackDamageToggle);
        root.Add(attackDamageFloatField);
        root.Add(critChanceToggle);
        root.Add(critChanceFloatField);
        root.Add(critDamageModifierToggle);
        root.Add(critDamageModifierFloatField);
        root.Add(button);
        rootVisualElement.Add(root);
    }
}
#endif
