using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Anything you want to change in the inspector, or to be
/// affected by other fields, should be placed here. for
/// convinience sake.
/// DON'T REMOVE SERIALIZEFIELD, THEY ARE MEAN TO BE PERSISTED.
/// </summary>
public partial class Stat : MonoEditor, INotifyBindablePropertyChanged
{
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    private void Awake()
    {
        customMono = GetComponent<CustomMono>();
        AddPropertyChangeEvent();
    }

    private void OnEnable()
    {
        onEnable();
    }

    private void OnDisable()
    {
        // if (healthRadialProgress != null)
        // {
        //     if (healthRadialProgress.gameEffect != null)
        //         healthRadialProgress.gameEffect.deactivate();
        // }
    }

    public override void Start()
    {
        InitUI();
        StartCoroutine(LateStart());
        onEnable += () =>
        {
            InitUI();
            StartCoroutine(LateStart());
            ResetStat();
        };
    }

    IEnumerator LateStart()
    {
        yield return null;
        InitProperty();
        ResetStat();
    }

    void ResetStat()
    {
        CurrentHealthPoint = HealthPoint;
        CurrentManaPoint = ManaPoint;
        MoveSpeed = DefaultMoveSpeed;
    }

    void InitUI()
    {
        healthBar = GameUIManagerRevamp.Instance.CreateAndHandleRadialProgressFollowing(transform);
    }

    public void InitProperty()
    {
        Notify("AttackSpeed");
        Notify("BaseAttackSpeed");
        Notify("CurrentHealthPoint");
        Notify("HealthPoint");
        Notify("CurrentManaPoint");
        Notify("ManaPoint");
        Notify("Might");
        Notify("BaseMight");
        Notify("Reflex");
        Notify("BaseReflex");
        Notify("Wisdom");
        Notify("BaseWisdom");
        Notify("MoveSpeed");
        Notify("DefaultMoveSpeed");
        Notify("ActionMoveSpeedReduceRate");
        Notify("Size");
    }

    void AddPropertyChangeEvent()
    {
        currentHealthPointChangeEvent.action += () =>
        {
            healthBar.healthUIRevamp.SetHealth(CurrentHealthPoint / HealthPoint);
        };

        actionMoveSpeedReduceRateChangeEvent.action += () =>
            actionMoveSpeedReduced = defaultMoveSpeed * actionMoveSpeedReduceRate;
        currentHealthPointReachZeroEvent.action += () =>
        {
            customMono.AnimatorWrapper.SetBool(dieBoolHash, true);
            customMono.combatCollision.SetActive(false);
            healthBar.worldSpaceUI.deactivate();
            healthBar = null;
            StartCoroutine(DissolveCoroutine());
        };

        baseAttackSpeedChangeEvent.action += DefaultAttackSpeedChange;
        attackSpeedAdditionModifierChangeEvent.action += DefaultAttackSpeedChange;
        attackSpeedMultiplicationModifierChangeEvent.action += DefaultAttackSpeedChange;
        baseHealthPointChangeEvent.action += DefaultHealthPointChange;
        healthPointAdditionModifierChangeEvent.action += DefaultHealthPointChange;
        healthPointMultiplicationModifierChangeEvent.action += DefaultHealthPointChange;
        baseManaPointChangeEvent.action += DefaultManaPointChange;
        manaPointAdditionModifierChangeEvent.action += DefaultManaPointChange;
        manaPointMultiplicationModifierChangeEvent.action += DefaultManaPointChange;
        baseMightChangeEvent.action += DefaultMightChange;
        mightAdditionModifierChangeEvent.action += DefaultMightChange;
        mightMultiplicationModifierChangeEvent.action += DefaultMightChange;
        baseReflexChangeEvent.action += DefaultReflexChange;
        reflexAdditionModifierChangeEvent.action += DefaultReflexChange;
        reflexMultiplicationModifierChangeEvent.action += DefaultReflexChange;
        baseWisdomChangeEvent.action += DefaultWisdomChange;
        wisdomAdditionModifierChangeEvent.action += DefaultWisdomChange;
        wisdomMultiplicationModifierChangeEvent.action += DefaultWisdomChange;

        mightChangeEvent.action += DefaultHealthPointChange;
        reflexChangeEvent.action += DefaultAttackSpeedChange;
        wisdomChangeEvent.action += DefaultManaPointChange;

        propertyChangeEventDictionary.Add("AttackSpeed", attackSpeedChangeEvent);
        propertyChangeEventDictionary.Add("BaseAttackSpeed", baseAttackSpeedChangeEvent);
        propertyChangeEventDictionary.Add(
            "AttackSpeedAdditionModifier",
            attackSpeedAdditionModifierChangeEvent
        );
        propertyChangeEventDictionary.Add(
            "AttackSpeedMultiplicationModifier",
            attackSpeedMultiplicationModifierChangeEvent
        );

        propertyChangeEventDictionary.Add("CurrentHealthPoint", currentHealthPointChangeEvent);
        propertyChangeEventDictionary.Add("HealthPoint", healthPointChangeEvent);
        propertyChangeEventDictionary.Add("BaseHealthPoint", baseHealthPointChangeEvent);
        propertyChangeEventDictionary.Add(
            "HealthPointAdditionModifier",
            healthPointAdditionModifierChangeEvent
        );
        propertyChangeEventDictionary.Add(
            "HealthPointMultiplicationModifier",
            healthPointMultiplicationModifierChangeEvent
        );

        propertyChangeEventDictionary.Add("CurrentManaPoint", currentManaPointChangeEvent);
        propertyChangeEventDictionary.Add("ManaPoint", manaPointChangeEvent);
        propertyChangeEventDictionary.Add("BaseManaPoint", baseManaPointChangeEvent);
        propertyChangeEventDictionary.Add(
            "ManaPointAdditionModifier",
            manaPointAdditionModifierChangeEvent
        );
        propertyChangeEventDictionary.Add(
            "ManaPointMultiplicationModifier",
            manaPointMultiplicationModifierChangeEvent
        );

        propertyChangeEventDictionary.Add("Might", mightChangeEvent);
        propertyChangeEventDictionary.Add("BaseMight", baseMightChangeEvent);
        propertyChangeEventDictionary.Add(
            "MightAdditionModifier",
            mightAdditionModifierChangeEvent
        );
        propertyChangeEventDictionary.Add(
            "MightMultiplicationModifier",
            mightMultiplicationModifierChangeEvent
        );
        propertyChangeEventDictionary.Add("Reflex", reflexChangeEvent);
        propertyChangeEventDictionary.Add("BaseReflex", baseReflexChangeEvent);
        propertyChangeEventDictionary.Add(
            "ReflexAdditionModifier",
            reflexAdditionModifierChangeEvent
        );
        propertyChangeEventDictionary.Add(
            "ReflexMultiplicationModifier",
            reflexMultiplicationModifierChangeEvent
        );
        propertyChangeEventDictionary.Add("Wisdom", wisdomChangeEvent);
        propertyChangeEventDictionary.Add("BaseWisdom", baseWisdomChangeEvent);
        propertyChangeEventDictionary.Add(
            "WisdomAdditionModifier",
            wisdomAdditionModifierChangeEvent
        );
        propertyChangeEventDictionary.Add(
            "WisdomMultiplicationModifier",
            wisdomMultiplicationModifierChangeEvent
        );

        propertyChangeEventDictionary.Add("MoveSpeed", moveSpeedChangeEvent);
        propertyChangeEventDictionary.Add("DefaultMoveSpeed", defaultMoveSpeedChangeEvent);
        propertyChangeEventDictionary.Add(
            "ActionMoveSpeedReduceRate",
            actionMoveSpeedReduceRateChangeEvent
        );
        propertyChangeEventDictionary.Add("Size", sizeChangeEvent);
        propertyChangeEventDictionary.Add("ManaRegen", manaRegenChangeEvent);
    }

    void DefaultAttackSpeedChange()
    {
        AttackSpeed =
            (BaseAttackSpeed + AttackSpeedAdditionModifier + 0.1f * Reflex)
            * AttackSpeedMultiplicationModifier;
    }

    void DefaultHealthPointChange()
    {
        HealthPoint =
            (BaseHealthPoint + HealthPointAdditionModifier + 19 * Might)
            * HealthPointMultiplicationModifier;
    }

    void DefaultManaPointChange()
    {
        ManaPoint =
            (BaseManaPoint + ManaPointAdditionModifier + 13 * Wisdom)
            * ManaPointMultiplicationModifier;
    }

    void DefaultMightChange()
    {
        Might = (BaseMight + MightAdditionModifier) * MightMultiplicationModifier;
    }

    void DefaultReflexChange()
    {
        Reflex = (BaseReflex + ReflexAdditionModifier) * ReflexMultiplicationModifier;
    }

    void DefaultWisdomChange()
    {
        Wisdom = (BaseWisdom + WisdomAdditionModifier) * WisdomMultiplicationModifier;
    }

    void RecalculateManaRegen()
    {
        ManaRegen = (BaseManaRegen + ManaRegenAdditionModifier) * ManaRegenMultiplicationModifier;
    }

    void Notify([CallerMemberName] string property = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        if (propertyChangeEventDictionary.TryGetValue(property, out notifyAW))
            notifyAW.action();
    }

    public void SetDefaultMoveSpeed() => MoveSpeed = DefaultMoveSpeed;

    IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(dissolveTime);

        GameEffect dieDissolveEffect = GameManager.Instance.dieDissolvePool.PickOne().gameEffect;
        dieDissolveEffect.animateObjects[0].spriteRenderer.transform.localScale = customMono
            .directionModifier
            .transform
            .localScale;
        dieDissolveEffect.animateObjects[0].spriteRenderer.sprite = customMono
            .spriteRenderer
            .sprite;
        dieDissolveEffect.animateObjects[0].spriteRenderer.transform.position = transform.position;
        customMono.deactivate();
    }

    private void OnValidate()
    {
        BaseManaPoint = 100f;
        ManaPointAdditionModifier = 0;
        ManaPointMultiplicationModifier = 1;
    }
}
