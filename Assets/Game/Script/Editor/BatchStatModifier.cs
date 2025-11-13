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

        Toggle hpRegenToggle = new();
        hpRegenToggle.label = "HP Regen";
        hpRegenToggle.value = false;

        FloatField hpRegenFloatField = new();
        hpRegenFloatField.label = "HP Regen";
        hpRegenFloatField.value = 0f;

        Toggle mpToggle = new();
        mpToggle.label = "MP";
        mpToggle.value = false;

        FloatField mpFloatField = new();
        mpFloatField.label = "MP";
        mpFloatField.value = 0f;

        Toggle mpRegenToggle = new();
        mpRegenToggle.label = "MP Regen";
        mpRegenToggle.value = false;

        FloatField mpRegenFloatField = new();
        mpRegenFloatField.label = "MP Regen";
        mpRegenFloatField.value = 0f;

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

        Toggle damageModifierToggle = new();
        damageModifierToggle.label = "Damage Modifier";
        damageModifierToggle.value = false;

        FloatField damageModifierFloatField = new();
        damageModifierFloatField.label = "Damage Modifier";
        damageModifierFloatField.value = 0f;

        Toggle omnivampToggle = new();
        omnivampToggle.label = "Omnivamp";
        omnivampToggle.value = false;

        FloatField omnivampFloatField = new();
        omnivampFloatField.label = "Omnivamp";
        omnivampFloatField.value = 0f;

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

        Toggle damageReductionToggle = new();
        damageReductionToggle.label = "Damage Reduction";
        damageReductionToggle.value = false;

        FloatField damageReductionFloatField = new();
        damageReductionFloatField.label = "Damage Reduction";
        damageReductionFloatField.value = 0f;

        Toggle attackRangeToggle = new();
        attackRangeToggle.label = "Attack Range";
        attackRangeToggle.value = false;

        FloatField attackRangeFloatField = new();
        attackRangeFloatField.label = "Attack Range";
        attackRangeFloatField.value = 0f;

        Button button = new();
        button.text = "Modify";
        button.clicked += () =>
        {
            listStatSO.stats.ForEach(stat =>
            {
                if (hpRegenToggle.value)
                    stat.healthRegen.BaseValue = hpRegenFloatField.value;
                if (hpToggle.value)
                    stat.healthPoint.BaseValue = hpFloatField.value;
                if (mpToggle.value)
                    stat.manaPoint.BaseValue = mpFloatField.value;
                if (mpRegenToggle.value)
                    stat.manaRegen.BaseValue = mpRegenFloatField.value;
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
                // if (damageModifierToggle.value)
                //     stat.damageModifier.BaseValue = damageModifierFloatField.value;
                if (omnivampToggle.value)
                    stat.omnivamp.BaseValue = omnivampFloatField.value;
                if (attackDamageToggle.value)
                    stat.attackDamage.BaseValue = attackDamageFloatField.value;
                if (critChanceToggle.value)
                    stat.critChance.BaseValue = critChanceFloatField.value;
                if (critDamageModifierToggle.value)
                    stat.critDamageModifier.BaseValue = critDamageModifierFloatField.value;
                if (damageReductionToggle.value)
                    stat.damageReduction.BaseValue = damageReductionFloatField.value;
                if (attackRangeToggle.value)
                    stat.attackRange.BaseValue = attackRangeFloatField.value;
                EditorUtility.SetDirty(stat);
            });
            AssetDatabase.SaveAssets();
        };

        root.Add(inspectorListView);
        root.Add(hpRegenToggle);
        root.Add(hpRegenFloatField);
        root.Add(hpToggle);
        root.Add(hpFloatField);
        root.Add(mpToggle);
        root.Add(mpFloatField);
        root.Add(mpRegenToggle);
        root.Add(mpRegenFloatField);
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
        root.Add(omnivampToggle);
        root.Add(omnivampFloatField);
        root.Add(attackDamageToggle);
        root.Add(attackDamageFloatField);
        root.Add(critChanceToggle);
        root.Add(critChanceFloatField);
        root.Add(critDamageModifierToggle);
        root.Add(critDamageModifierFloatField);
        root.Add(damageReductionToggle);
        root.Add(damageReductionFloatField);
        root.Add(attackRangeToggle);
        root.Add(attackRangeFloatField);
        root.Add(button);
        rootVisualElement.Add(root);
    }
}
#endif
