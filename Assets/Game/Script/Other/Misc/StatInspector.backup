#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Stat))]
public class StatInspector : Editor
{
    Stat stat;

    [SerializeField]
    private VisualTreeAsset statInspectorAsset;
    VisualElement root;
    Slider defaultMoveSpeedSlider,
        actionMoveSpeedReduceRateSlider,
        sizeSlider;
    Label moveSpeedLabel,
        defaultMoveSpeedLabel,
        actionMoveSpeedReduceRateLabel,
        sizeLabel;
    FloatField attackSpeedFloatField,
        baseAttackSpeedFloatField,
        currentHealthPointFloatField,
        healthPointFloatField,
        baseHealthPointFloatField,
        currentManaPointFloatField,
        manaPointFloatField,
        baseManaPointFloatField,
        mightFloatField,
        baseMightFloatField,
        reflexFloatField,
        baseReflexFloatField,
        wisdomFloatField,
        baseWisdomFloatField;

    private void OnEnable()
    {
        stat = (Stat)target;
        root = statInspectorAsset.Instantiate();
        attackSpeedFloatField = root.Q<FloatField>("inspector__attack-speed-float-field");
        baseAttackSpeedFloatField = root.Q<FloatField>("inspector__base-attack-speed-float-field");
        currentHealthPointFloatField = root.Q<FloatField>(
            "inspector__current-health-point-float-field"
        );
        healthPointFloatField = root.Q<FloatField>("inspector__health-point-float-field");
        baseHealthPointFloatField = root.Q<FloatField>("inspector__base-health-point-float-field");
        currentManaPointFloatField = root.Q<FloatField>(
            "inspector__current-mana-point-float-field"
        );
        manaPointFloatField = root.Q<FloatField>("inspector__mana-point-float-field");
        baseManaPointFloatField = root.Q<FloatField>("inspector__base-mana-point-float-field");
        moveSpeedLabel = root.Q<Label>(name: "inspector__move-speed-label");
        defaultMoveSpeedSlider = root.Q<Slider>(name: "inspector__default-move-speed-slider");
        defaultMoveSpeedLabel = root.Q<Label>(name: "inspector__default-move-speed-label");
        actionMoveSpeedReduceRateSlider = root.Q<Slider>(
            name: "inspector__action-move-speed-reduce-rate-slider"
        );
        actionMoveSpeedReduceRateLabel = root.Q<Label>(
            name: "inspector__action-move-speed-reduce-rate-label"
        );
        sizeSlider = root.Q<Slider>(name: "inspector__size-slider");
        sizeLabel = root.Q<Label>(name: "inspector__size-label");
        mightFloatField = root.Q<FloatField>(name: "inspector__might-float-field");
        baseMightFloatField = root.Q<FloatField>(name: "inspector__base-might-float-field");
        reflexFloatField = root.Q<FloatField>(name: "inspector__reflex-float-field");
        baseReflexFloatField = root.Q<FloatField>(name: "inspector__base-reflex-float-field");
        wisdomFloatField = root.Q<FloatField>(name: "inspector__wisdom-float-field");
        baseWisdomFloatField = root.Q<FloatField>(name: "inspector__base-wisdom-float-field");

        defaultMoveSpeedSlider.lowValue =
            actionMoveSpeedReduceRateSlider.lowValue =
            sizeSlider.lowValue =
                0;
        sizeSlider.highValue = 5f;
        defaultMoveSpeedSlider.highValue = 100;
        actionMoveSpeedReduceRateSlider.highValue = 10f;

        root.dataSource = stat;

        attackSpeedFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.AttackSpeed)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        baseAttackSpeedFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.BaseAttackSpeed)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        currentHealthPointFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.CurrentHealthPoint)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        healthPointFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.HealthPoint)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        baseHealthPointFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.BaseHealthPoint)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        currentManaPointFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.CurrentManaPoint)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        manaPointFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.ManaPoint)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        baseManaPointFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.BaseManaPoint)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        moveSpeedLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.MoveSpeed));
        var moveSpeedBinding = new DataBinding() { bindingMode = BindingMode.ToTarget };
        moveSpeedBinding.sourceToUiConverters.AddConverter(
            (ref float value) =>
            {
                return "Move Speed " + value + " 👟";
            }
        );
        moveSpeedLabel.SetBinding("text", moveSpeedBinding);

        defaultMoveSpeedSlider.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultMoveSpeed)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        defaultMoveSpeedLabel.dataSourcePath = new Unity.Properties.PropertyPath(
            nameof(Stat.DefaultMoveSpeed)
        );
        var defaultMoveSpeedBinding = new DataBinding() { bindingMode = BindingMode.ToTarget };
        defaultMoveSpeedBinding.sourceToUiConverters.AddConverter(
            (ref float value) =>
            {
                return value + " 👢";
            }
        );
        defaultMoveSpeedLabel.SetBinding("text", defaultMoveSpeedBinding);

        actionMoveSpeedReduceRateSlider.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(
                    nameof(Stat.ActionMoveSpeedReduceRate)
                ),
                bindingMode = BindingMode.TwoWay,
            }
        );

        actionMoveSpeedReduceRateLabel.dataSourcePath = new Unity.Properties.PropertyPath(
            nameof(Stat.ActionMoveSpeedReduceRate)
        );
        var actionMoveSpeedReduceRateBinding = new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
        };
        actionMoveSpeedReduceRateBinding.sourceToUiConverters.AddConverter(
            (ref float value) =>
            {
                return value + " 👠";
            }
        );
        actionMoveSpeedReduceRateLabel.SetBinding("text", actionMoveSpeedReduceRateBinding);

        sizeSlider.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Size)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        sizeLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Size));
        var sizeBinding = new DataBinding() { bindingMode = BindingMode.ToTarget };
        sizeBinding.sourceToUiConverters.AddConverter(
            (ref float value) =>
            {
                return value + " ☩";
            }
        );
        sizeLabel.SetBinding("text", sizeBinding);

        mightFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Might)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        baseMightFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.BaseMight)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        reflexFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Reflex)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        baseReflexFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.BaseReflex)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        wisdomFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Wisdom)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        baseWisdomFloatField.SetBinding(
            "value",
            new DataBinding()
            {
                dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.BaseWisdom)),
                bindingMode = BindingMode.TwoWay,
            }
        );

        Redraw();
    }

    void Redraw()
    {
        // baseMightFloatField.RegisterValueChangedCallback(
        //     (b) =>
        //     {
        //         Repaint();
        //     }
        // );
    }

    public override VisualElement CreateInspectorGUI()
    {
        return root;
    }
}

#endif
