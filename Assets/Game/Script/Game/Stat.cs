using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Contains all the stats of the champion like HP, MP, ...
/// </summary>
[DefaultExecutionOrder(0)]
public partial class Stat : MonoBehaviour
{
    private void Awake()
    {
        customMono = GetComponent<CustomMono>();
        AddPropertyChangeEvent();
    }

    private void OnEnable()
    {
        onEnable();
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        StartRegen();
    }

    public void Start()
    {
        SetupGameEvent();
        customMono.scriptExecutionCore.lateEnable0 += CalculateFinalStats;
    }

    public void SetupForReuseIE()
    {
        ResetFields();
        InitUI();
    }

    private void CalculateFinalStats()
    {
        /* Don't need to recalculate hp, hpregen, mp, mpregen, aspd, armor since
        other stats will recalculate them (for example see this method
        MightChangeHealthPoint) */
        might.RecalculateFinalValue();
        reflex.RecalculateFinalValue();
        wisdom.RecalculateFinalValue();
        moveSpeed.RecalculateFinalValue();
        damageModifier.RecalculateFinalValue();
        omnivamp.RecalculateFinalValue();
        attackDamage.RecalculateFinalValue();
        critChance.RecalculateFinalValue();
        critDamageModifier.RecalculateFinalValue();
        damageReduction.RecalculateFinalValue();
        attackRange.RecalculateFinalValue();

        currentHealthPoint.Value = healthPoint.FinalValue;
        currentManaPoint.Value = 0;
    }

    /// <summary>
    /// Clear all stat modifiers, recalculation will be done in LateEnable
    /// </summary>
    void ResetFields()
    {
        alive = true;
        healthPoint.ClearModifiersWithoutRecalculate(customMono.championData.healthPoint);
        healthRegen.ClearModifiersWithoutRecalculate(customMono.championData.healthRegen);
        manaPoint.ClearModifiersWithoutRecalculate(customMono.championData.manaPoint);
        manaRegen.ClearModifiersWithoutRecalculate(customMono.championData.manaRegen);
        might.ClearModifiersWithoutRecalculate(customMono.championData.might);
        reflex.ClearModifiersWithoutRecalculate(customMono.championData.reflex);
        wisdom.ClearModifiersWithoutRecalculate(customMono.championData.wisdom);
        attackSpeed.ClearModifiersWithoutRecalculate(customMono.championData.attackSpeed);
        armor.ClearModifiersWithoutRecalculate(customMono.championData.armor);
        moveSpeed.ClearModifiersWithoutRecalculate(customMono.championData.moveSpeed);
        damageModifier.ClearModifiersWithoutRecalculate(customMono.championData.damageModifier);
        omnivamp.ClearModifiersWithoutRecalculate(customMono.championData.omnivamp);
        attackDamage.ClearModifiersWithoutRecalculate(customMono.championData.attackDamage);
        critChance.ClearModifiersWithoutRecalculate(customMono.championData.critChance);
        critDamageModifier.ClearModifiersWithoutRecalculate(
            customMono.championData.critDamageModifier
        );
        damageReduction.ClearModifiersWithoutRecalculate(customMono.championData.damageReduction);
        attackRange.ClearModifiersWithoutRecalculate(customMono.championData.attackRange);
        actionMoveSpeedReduceRate.ClearModifiersWithoutRecalculate();
    }

    void InitUI()
    {
        healthAndManaIndicatorPO = GameUIManager.Instance.CreateAndHandleHPAndMPUIWithFollowing(
            transform
        );
    }

    void AddPropertyChangeEvent()
    {
        currentHealthPoint.valueChangeEvent += HPChangeCallback;
        currentManaPoint.valueChangeEvent += SetMPOnUI;
        healthPoint.finalValueChangeEvent += ChangeCurrentHPCap;
        manaPoint.finalValueChangeEvent += ChangeCurrentMPCap;

        healthRegen.modifiers.Add(mightChangeHealthRegenFSM);
        healthPoint.modifiers.Add(mightChangeHealthPointFSM);
        might.finalValueChangeEvent += MightChangeHealthPoint;
        might.finalValueChangeEvent += MightChangeHealthRegen;

        attackSpeed.modifiers.Add(reflexChangeAttackSpeedFSM);
        armor.modifiers.Add(reflexChangeArmorFSM);
        reflex.finalValueChangeEvent += ReflexChangeAttackSpeed;
        reflex.finalValueChangeEvent += ReflexChangeArmor;

        manaRegen.modifiers.Add(wisdomChangeManaRegenFSM);
        manaPoint.modifiers.Add(wisdomChangeManaPointFSM);
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
            currentHealthPointReachZeroEvent(customMono);
            HealthReachZeroHandler();
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
        customMono.actionBlocking = true;
        customMono.movementActionBlocking = true;
        customMono.AnimatorWrapper.SetBool(GameManager.Instance.dieBoolHash, true);
        customMono.combatCollision.SetActive(false);
        healthAndManaIndicatorPO?.WorldSpaceUI.deactivate();
        healthAndManaIndicatorPO = null;
        StopAllCoroutines();
        StartCoroutine(DissolveCoroutine());
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
        mightChangeHealthPointFSM.value = 15 * might.FinalValue;
        healthPoint.RecalculateFinalValue();
    }

    /// <summary>
    /// HealthRegen =
    ///        (BaseHealthRegen + HealthRegenAdditionModifier + 0.1f * Might)
    ///        * HealthRegenMultiplicationModifier;
    /// </summary>
    void MightChangeHealthRegen()
    {
        mightChangeHealthRegenFSM.value = 0.1f * might.FinalValue;
        healthRegen.RecalculateFinalValue();
    }

    /// <summary>
    /// AttackSpeed =
    ///    (BaseAttackSpeed + AttackSpeedAdditionModifier + 0.01f * Reflex)
    ///     * AttackSpeedMultiplicationModifier;
    /// </summary>
    void ReflexChangeAttackSpeed()
    {
        reflexChangeAttackSpeedFSM.value = 0.01f * reflex.FinalValue;
        attackSpeed.RecalculateFinalValue();
    }

    /// <summary>
    /// Armor =
    ///    (BaseArmor + ArmorAdditionModifier + 0.17f * Reflex)
    ///     * ArmorMultiplicationModifier;
    /// </summary>
    void ReflexChangeArmor()
    {
        reflexChangeArmorFSM.value = 0.17f * reflex.FinalValue;
        armor.RecalculateFinalValue();
    }

    /// <summary>
    /// ManaPoint =
    ///    (BaseManaPoint + ManaPointAdditionModifier - 0.25 * Wisdom)
    ///    * ManaPointMultiplicationModifier;
    /// </summary>
    void WisdomChangeManaPoint()
    {
        wisdomChangeManaPointFSM.value = -0.25f * wisdom.FinalValue;
        manaPoint.RecalculateFinalValue();
    }

    /// <summary>
    /// ManaRegen =
    ///        (BaseManaRegen + ManaRegenAdditionModifier + 1.5 * Wisdom)
    ///        * ManaRegenMultiplicationModifier;
    /// </summary>
    void WisdomChangeManaRegen()
    {
        wisdomChangeManaRegenFSM.value = 1.5f * wisdom.FinalValue;
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
        //
    }

    private void Reset()
    {
        //
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

    public bool EquipItem(Item item)
    {
        if (equippedItems.Count >= itemSlot.FinalValue)
            return false;

        item.itemDataSO.statBuffs.ForEach(sB =>
        {
            GameManager.Instance.AddBuff(customMono, sB);
        });

        item.itemDataSO.itemBehaviours.ForEach(iB =>
        {
            var behaviorComp =
                gameObject.AddComponent(GameManager.Instance.MapItemBehaviour(iB))
                as IItemBehaviour;
            behaviorComp.OnAttach(customMono, item);
        });

        GameUIManager.Instance.GetChampInfoPanel(customMono).AttachItem(item);
        equippedItems.Add(item);
        return true;
    }

    public void UnEquipItem(Item item)
    {
        item.itemDataSO.statBuffs.ForEach(sB =>
        {
            GameManager.Instance.RemoveBuff(customMono, sB);
        });

        item.itemDataSO.itemBehaviours.ForEach(iB =>
        {
            var behaviorComp =
                gameObject.GetComponent(GameManager.Instance.MapItemBehaviour(iB))
                as IItemBehaviour;
            behaviorComp.OnDetach();
            Destroy((MonoBehaviour)behaviorComp);
        });

        GameUIManager.Instance.GetChampInfoPanel(customMono).DetachItem(item);
        equippedItems.Remove(item);
    }

    /// <summary>
    /// Hide health and mana UI
    /// </summary>
    public void Hide()
    {
        healthAndManaIndicatorPO.gameObject.SetActive(false);
    }

    public void Reveal() => Show();

    public void Show()
    {
        healthAndManaIndicatorPO.gameObject.SetActive(true);
        GameUIManager.Instance.ReHandleHPAndMPUIWithFollowing(healthAndManaIndicatorPO, transform);
    }
}
