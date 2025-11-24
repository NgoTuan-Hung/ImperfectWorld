using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Anything you want to change in the inspector, or to be
/// affected by other fields, should be placed here. for
/// convinience sake.
/// DON'T REMOVE SERIALIZEFIELD, THEY ARE MEAN TO BE PERSISTED.
/// </summary>
public partial class Stat : MonoEditor
{
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
        SetupGameEvent();
    }

    IEnumerator LateStart()
    {
        yield return null;
        InitProperty();
        ResetStat();
        StartRegen();
    }

    /// <summary>
    /// Since we cripple the character on death, we need to
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
        currentManaPoint.Value = 0;
    }

    void InitUI()
    {
        healthAndManaIndicatorPO = GameUIManager.Instance.CreateAndHandleHPAndMPUIWithFollowing(
            transform
        );
    }

    public void InitProperty()
    {
        alive = true;
        might.RecalculateFinalValue();
        reflex.RecalculateFinalValue();
        wisdom.RecalculateFinalValue();
        actionMoveSpeedReduceRate.RecalculateFinalValue();
        moveSpeed.RecalculateFinalValue();
        damageModifier.RecalculateFinalValue();
        armor.RecalculateFinalValue();
        omnivamp.RecalculateFinalValue();
        attackDamage.RecalculateFinalValue();
        critChance.RecalculateFinalValue();
        critDamageModifier.RecalculateFinalValue();
        attackRange.RecalculateFinalValue();
    }

    void AddPropertyChangeEvent()
    {
        currentHealthPoint.valueChangeEvent += HPChangeCallback;
        currentManaPoint.valueChangeEvent += SetMPOnUI;
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

    private void SetupGameEvent()
    {
        hpChangeED = new(customMono);
        currentHealthPoint.valueChangeEvent += () =>
        {
            hpChangeED.currentValue = currentHealthPoint.Value;
            hpChangeED.maxValue = healthPoint.FinalValue;
            GameManager
                .Instance.GetTeamBasedEvent(customMono.tag, GameEventType.HPChange)
                .action(hpChangeED);
        };

        GameManager.Instance.GetSelfEvent(customMono, GameEventType.DealDamage).action +=
            OnDealDamage;
    }

    void HPChangeCallback()
    {
        SetHPOnUI();
        beforeDeathCallback();
        if (currentHealthPoint.Value <= 0 && alive)
        {
            alive = false;
            currentHealthPointReachZeroEvent();
        }
    }

    void SetHPOnUI() =>
        healthAndManaIndicatorPO?.HealthAndManaIndicator.SetHealth(
            currentHealthPoint.Value / healthPoint.FinalValue
        );

    void SetMPOnUI() =>
        healthAndManaIndicatorPO?.HealthAndManaIndicator.SetMana(
            currentManaPoint.Value / manaPoint.FinalValue
        );

    void HealthReachZeroHandler()
    {
        if (currentHealthPoint.Value <= 0)
        {
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            customMono.AnimatorWrapper.SetBool(GameManager.Instance.dieBoolHash, true);
            customMono.combatCollision.SetActive(false);
            healthAndManaIndicatorPO?.WorldSpaceUI.deactivate();
            healthAndManaIndicatorPO = null;
            StopAllCoroutines();
            StartCoroutine(DissolveCoroutine());
        }
    }

    void ChangeCurrentHPCap() => currentHealthPoint.cap = healthPoint.FinalValue;

    void ChangeCurrentMPCap() => currentManaPoint.cap = manaPoint.FinalValue;

    /// <summary>
    /// HealthPoint =
    ///    (BaseHealthPoint + HealthPointAdditionModifier + 15 * Might)
    ///     * HealthPointMultiplicationModifier;
    /// </summary>
    void MightChangeHealthPoint()
    {
        might.referenceModifiers[1].value = 15 * might.FinalValue;
        healthPoint.RecalculateFinalValue();
    }

    /// <summary>
    /// HealthRegen =
    ///        (BaseHealthRegen + HealthRegenAdditionModifier + 0.1f * Might)
    ///        * HealthRegenMultiplicationModifier;
    /// </summary>
    void MightChangeHealthRegen()
    {
        might.referenceModifiers[0].value = 0.1f * might.FinalValue;
        healthRegen.RecalculateFinalValue();
    }

    /// <summary>
    /// AttackSpeed =
    ///    (BaseAttackSpeed + AttackSpeedAdditionModifier + 0.01f * Reflex)
    ///     * AttackSpeedMultiplicationModifier;
    /// </summary>
    void ReflexChangeAttackSpeed()
    {
        reflex.referenceModifiers[0].value = 0.01f * reflex.FinalValue;
        attackSpeed.RecalculateFinalValue();
    }

    /// <summary>
    /// Armor =
    ///    (BaseArmor + ArmorAdditionModifier + 0.17f * Reflex)
    ///     * ArmorMultiplicationModifier;
    /// </summary>
    void ReflexChangeArmor()
    {
        reflex.referenceModifiers[1].value = 0.17f * reflex.FinalValue;
        armor.RecalculateFinalValue();
    }

    /// <summary>
    /// ManaPoint =
    ///    (BaseManaPoint + ManaPointAdditionModifier - 0.25 * Wisdom)
    ///    * ManaPointMultiplicationModifier;
    /// </summary>
    void WisdomChangeManaPoint()
    {
        wisdom.referenceModifiers[1].value = -0.25f * wisdom.FinalValue;
        manaPoint.RecalculateFinalValue();
    }

    /// <summary>
    /// ManaRegen =
    ///        (BaseManaRegen + ManaRegenAdditionModifier + 1.5 * Wisdom)
    ///        * ManaRegenMultiplicationModifier;
    /// </summary>
    void WisdomChangeManaRegen()
    {
        wisdom.referenceModifiers[0].value = 1.5f * wisdom.FinalValue;
        manaRegen.RecalculateFinalValue();
    }

    void MoveSpeedChangeMoveSpeedPerFrame() =>
        moveSpeedPerFrame = moveSpeed.FinalValue * Time.fixedDeltaTime;

    void ChangeActionSlow() => actionSlowModifier.value = actionMoveSpeedReduceRate.FinalValue;

    IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(dissolveTime);

        GameEffect dieDissolveEffect = GameManager.Instance.dieDissolvePool.PickOneGameEffect();
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
            if (canRegen)
            {
                currentManaPoint.Value += manaRegen.FinalValue * Time.deltaTime;
                currentHealthPoint.Value += healthRegen.FinalValue * Time.deltaTime;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Used to handle mechanic like Omnivamp, ...
    /// </summary>
    /// <param name="p_gED"></param>U
    public void OnDealDamage(IGameEventData p_gED)
    {
        dealDamageGameEventData = p_gED.As<DealDamageGameEventData>();

        /* Omnivamp */
        currentHealthPoint.Value += dealDamageGameEventData.damage * omnivamp.FinalValue;
    }

    /// <summary>
    /// Return final damage value considering all damage modifiers (Buff, Debuff, etc.)
    /// </summary>
    /// <param name="p_damage"></param>
    /// <returns></returns>
    public float CalculateDamageDeal(float damage) => damage * damageModifier.FinalValue;

    public void EnableRegen() => canRegen = true;

    public void DisableRegen() => canRegen = false;
}
