using System;
using System.Collections;
using System.Collections.Generic;
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
        ReviveHandler();
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
        StartCoroutine(AddLatePropertyChangeEvent());
        onEnable += () =>
        {
            InitUI();
            StartCoroutine(LateStart());
        };
    }

    IEnumerator LateStart()
    {
        yield return null;
        InitProperty();
        ResetStat();
        StartRegen();
    }

    /// <summary>
    /// Since we cripple the entity on death, we need to
    /// reset it state when it's re initialized.
    /// </summary>
    public void ReviveHandler()
    {
        customMono.actionBlocking = false;
        customMono.actionInterval = false;
        customMono.movementActionInterval = false;
        customMono.movementActionBlocking = false;
    }

    /// <summary>
    /// HealthReachZeroHandler should run after other registered events
    /// in BaseAction.
    /// </summary>
    /// <returns></returns>
    IEnumerator AddLatePropertyChangeEvent()
    {
        yield return null;
        currentHealthPointReachZeroEvent += HealthReachZeroHandler;
    }

    void ResetStat()
    {
        currentHealthPoint.Value = healthPoint.FinalValue;
        currentManaPoint.Value = manaPoint.FinalValue;
    }

    void InitUI()
    {
        healthAndManaIndicatorPO =
            GameUIManagerRevamp.Instance.CreateAndHandleHPAndMPUIWithFollowing(transform);
    }

    public void InitProperty()
    {
        might.RecalculateFinalValue();
        reflex.RecalculateFinalValue();
        wisdom.RecalculateFinalValue();
        actionMoveSpeedReduceRate.RecalculateFinalValue();

        Notify("Size");
    }

    void AddPropertyChangeEvent()
    {
        currentHealthPoint.valueChangeEvent += SetHPOnUI;
        currentManaPoint.valueChangeEvent += SetMPOnUI;
        currentHealthPoint.valueChangeEvent += CheckCurrentHPBelowZero;
        healthPoint.finalValueChangeEvent += ChangeCurrentHPCap;
        manaPoint.finalValueChangeEvent += ChangeCurrentMPCap;

        might.referenceModifiers = new List<FloatStatModifier>()
        {
            new(0, FloatStatModifierType.Additive),
            new(0, FloatStatModifierType.Additive),
        };
        healthRegen.modifiers.Add(might.referenceModifiers[0]);
        healthPoint.modifiers.Add(might.referenceModifiers[1]);
        might.finalValueChangeEvent += MightChangeHealthPoint;
        might.finalValueChangeEvent += MightChangeHealthRegen;

        reflex.referenceModifiers = new List<FloatStatModifier>()
        {
            new(0, FloatStatModifierType.Additive),
            new(0, FloatStatModifierType.Additive),
        };
        attackSpeed.modifiers.Add(reflex.referenceModifiers[0]);
        armor.modifiers.Add(reflex.referenceModifiers[1]);
        reflex.finalValueChangeEvent += ReflexChangeAttackSpeed;
        reflex.finalValueChangeEvent += ReflexChangeArmor;

        wisdom.referenceModifiers = new List<FloatStatModifier>()
        {
            new(0, FloatStatModifierType.Additive),
            new(0, FloatStatModifierType.Additive),
        };
        manaRegen.modifiers.Add(wisdom.referenceModifiers[0]);
        manaPoint.modifiers.Add(wisdom.referenceModifiers[1]);
        wisdom.finalValueChangeEvent += WisdomChangeManaPoint;
        wisdom.finalValueChangeEvent += WisdomChangeManaRegen;

        moveSpeed.finalValueChangeEvent += MoveSpeedChangeMoveSpeedPerFrame;
        actionMoveSpeedReduceRate.finalValueChangeEvent += ChangeActionSlow;
    }

    void CheckCurrentHPBelowZero()
    {
        if (currentHealthPoint.Value <= 0)
            currentHealthPointReachZeroEvent();
    }

    void SetHPOnUI() =>
        healthAndManaIndicatorPO.healthAndManaIndicator.SetHealth(
            currentHealthPoint.Value / healthPoint.FinalValue
        );

    void SetMPOnUI() =>
        healthAndManaIndicatorPO.healthAndManaIndicator.SetMana(
            currentManaPoint.Value / manaPoint.FinalValue
        );

    void HealthReachZeroHandler()
    {
        if (currentHealthPoint.Value <= 0)
        {
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            customMono.AnimatorWrapper.SetBool(dieBoolHash, true);
            customMono.combatCollision.SetActive(false);
            healthAndManaIndicatorPO.worldSpaceUI.deactivate();
            healthAndManaIndicatorPO = null;
            StopAllCoroutines();
            StartCoroutine(DissolveCoroutine());
        }
    }

    void ChangeCurrentHPCap() => currentHealthPoint.cap = healthPoint.FinalValue;

    void ChangeCurrentMPCap() => currentManaPoint.cap = manaPoint.FinalValue;

    /// <summary>
    /// HealthPoint =
    ///    (BaseHealthPoint + HealthPointAdditionModifier + 19 * Might)
    ///     * HealthPointMultiplicationModifier;
    /// </summary>
    void MightChangeHealthPoint()
    {
        might.referenceModifiers[1].value = 19 * might.FinalValue;
        healthPoint.RecalculateFinalValue();
    }

    /// <summary>
    /// HealthRegen =
    ///        (BaseHealthRegen + HealthRegenAdditionModifier + 0.03f * Might)
    ///        * HealthRegenMultiplicationModifier;
    /// </summary>
    void MightChangeHealthRegen()
    {
        might.referenceModifiers[0].value = 0.03f * might.FinalValue;
        healthRegen.RecalculateFinalValue();
    }

    /// <summary>
    /// AttackSpeed =
    ///    (BaseAttackSpeed + AttackSpeedAdditionModifier + 0.1f * Reflex)
    ///     * AttackSpeedMultiplicationModifier;
    /// </summary>
    void ReflexChangeAttackSpeed()
    {
        reflex.referenceModifiers[0].value = 0.1f * reflex.FinalValue;
        attackSpeed.RecalculateFinalValue();
    }

    /// <summary>
    /// Armor =
    ///    (BaseArmor + ArmorAdditionModifier + 0.1f * Reflex)
    ///     * ArmorMultiplicationModifier;
    /// </summary>
    void ReflexChangeArmor()
    {
        reflex.referenceModifiers[1].value = 0.1f * reflex.FinalValue;
        armor.RecalculateFinalValue();
    }

    /// <summary>
    /// ManaPoint =
    ///    (BaseManaPoint + ManaPointAdditionModifier + 13 * Wisdom)
    ///    * ManaPointMultiplicationModifier;
    /// </summary>
    void WisdomChangeManaPoint()
    {
        wisdom.referenceModifiers[1].value = 13 * wisdom.FinalValue;
        manaPoint.RecalculateFinalValue();
    }

    /// <summary>
    /// ManaRegen =
    ///        (BaseManaRegen + ManaRegenAdditionModifier + 0.04f * Wisdom)
    ///        * ManaRegenMultiplicationModifier;
    /// </summary>
    void WisdomChangeManaRegen()
    {
        wisdom.referenceModifiers[0].value = 0.04f * wisdom.FinalValue;
        manaRegen.RecalculateFinalValue();
    }

    void MoveSpeedChangeMoveSpeedPerFrame() =>
        moveSpeedPerFrame = moveSpeed.FinalValue * Time.fixedDeltaTime;

    void ChangeActionSlow() => actionSlowModifier.value = actionMoveSpeedReduceRate.FinalValue;

    void Notify([CallerMemberName] string property = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        if (propertyChangeEventDictionary.TryGetValue(property, out notifyAW))
            notifyAW.action();
    }

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
        // attackSpeed.baseValue = 1;
        // healthPoint.BaseValue = 20;
        // might.baseValue = 1;
        // reflex.baseValue = 1;
        // wisdom.baseValue = 1;
        // manaPoint.baseValue = 100;
        // manaRegen.baseValue = 0;
        // healthRegen.baseValue = 0;
        // armor.baseValue = 0;

        // moveSpeed.BaseValue = 2f;
        // actionMoveSpeedReduceRate.BaseValue = -0.9f;
    }

    private void Reset()
    {
        // attackSpeed.BaseValue = 1;
        // healthPoint.BaseValue = 20;
        // might.BaseValue = 1;
        // reflex.BaseValue = 1;
        // wisdom.BaseValue = 1;
        // manaPoint.BaseValue = 100;
        // manaRegen.BaseValue = 0;
        // healthRegen.BaseValue = 0;
        // armor.BaseValue = 0;

        // moveSpeed.BaseValue = 2f;
        // actionMoveSpeedReduceRate.BaseValue = -0.9f; //
    }

    void StartRegen()
    {
        StartCoroutine(RegenIE());
    }

    IEnumerator RegenIE()
    {
        while (true)
        {
            currentManaPoint.Value += manaRegen.FinalValue * Time.deltaTime;
            currentHealthPoint.Value += healthRegen.FinalValue * Time.deltaTime;
            yield return null;
        }
    }
}
