using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public partial class Stat
{
    CustomMono customMono;
    private PoolObject healthBar;

    [SerializeField]
    float attackSpeed = 1;

    [SerializeField]
    float baseAttackSpeed;

    [SerializeField]
    float attackSpeedAdditionModifier = 0;

    [SerializeField]
    float attackSpeedMultiplicationModifier = 1;

    [SerializeField]
    float moveSpeed = 1f;

    [SerializeField]
    private float defaultMoveSpeed;

    [SerializeField]
    float actionMoveSpeedReduceRate = 0.9f;
    public float actionMoveSpeedReduced;
    public float moveSpeedPerFrame;

    [SerializeField]
    float size = 1f;
    float dissolveTime = 5f;
    int dieBoolHash = Animator.StringToHash("Die");
    Action onEnable = () => { };

    [SerializeField]
    float currentHealthPoint = 100f;

    [SerializeField]
    float healthPoint = 50;

    [SerializeField]
    float baseHealthPoint = 50;

    [SerializeField]
    float healthPointAdditionModifier = 0;

    [SerializeField]
    float healthPointMultiplicationModifier = 1;

    [SerializeField]
    float might = 1f;

    [SerializeField]
    float baseMight = 1;

    [SerializeField]
    float mightAdditionModifier = 0;

    [SerializeField]
    float mightMultiplicationModifier = 1;

    [SerializeField]
    float reflex = 1f;

    [SerializeField]
    float baseReflex = 1;

    [SerializeField]
    float reflexAdditionModifier = 0;

    [SerializeField]
    float reflexMultiplicationModifier = 1;

    [SerializeField]
    float wisdom = 1f;

    [SerializeField]
    float baseWisdom = 1;

    [SerializeField]
    float wisdomAdditionModifier = 0;

    [SerializeField]
    float wisdomMultiplicationModifier = 1;
    float currentManaPoint;

    [SerializeField]
    float manaPoint;

    [SerializeField]
    float baseManaPoint = 100f;

    [SerializeField]
    float manaPointAdditionModifier;

    [SerializeField]
    float manaPointMultiplicationModifier;

    [CreateProperty]
    public float AttackSpeed
    {
        get => attackSpeed;
        set
        {
            if (value == attackSpeed)
                return;

            attackSpeed = value;
            Notify();
        }
    }
    public ActionWrapper attackSpeedChangeEvent = new();

    [CreateProperty]
    public float BaseAttackSpeed
    {
        get => baseAttackSpeed;
        set
        {
            if (value == baseAttackSpeed)
                return;

            baseAttackSpeed = value;
            Notify();
        }
    }
    public ActionWrapper baseAttackSpeedChangeEvent = new();

    [CreateProperty]
    public float AttackSpeedAdditionModifier
    {
        get => attackSpeedAdditionModifier;
        set
        {
            if (value == attackSpeedAdditionModifier)
                return;

            attackSpeedAdditionModifier = value;
            Notify();
        }
    }
    public ActionWrapper attackSpeedAdditionModifierChangeEvent = new();

    [CreateProperty]
    public float AttackSpeedMultiplicationModifier
    {
        get => attackSpeedMultiplicationModifier;
        set
        {
            if (value == attackSpeedMultiplicationModifier)
                return;

            attackSpeedMultiplicationModifier = value;
            Notify();
        }
    }
    public ActionWrapper attackSpeedMultiplicationModifierChangeEvent = new();

    [CreateProperty]
    public float CurrentHealthPoint
    {
        get => currentHealthPoint;
        set
        {
            if (value == currentHealthPoint)
                return;

            if (value > HealthPoint)
                currentHealthPoint = HealthPoint;
            else
                currentHealthPoint = value;
            Notify();
            if (currentHealthPoint <= 0)
                currentHealthPointReachZeroEvent.action();
        }
    }
    public ActionWrapper currentHealthPointChangeEvent = new();
    public ActionWrapper currentHealthPointReachZeroEvent = new();

    [CreateProperty]
    public float HealthPoint
    {
        get => healthPoint;
        set
        {
            if (value == healthPoint)
                return;

            healthPoint = value;
            Notify();
        }
    }
    public ActionWrapper healthPointChangeEvent = new();

    [CreateProperty]
    public float BaseHealthPoint
    {
        get => baseHealthPoint;
        set
        {
            if (value == baseHealthPoint)
                return;

            baseHealthPoint = value;
            Notify();
        }
    }
    public ActionWrapper baseHealthPointChangeEvent = new();

    [CreateProperty]
    public float HealthPointAdditionModifier
    {
        get => healthPointAdditionModifier;
        set
        {
            if (value == healthPointAdditionModifier)
                return;

            healthPointAdditionModifier = value;
            Notify();
        }
    }
    public ActionWrapper healthPointAdditionModifierChangeEvent = new();

    [CreateProperty]
    public float HealthPointMultiplicationModifier
    {
        get => healthPointMultiplicationModifier;
        set
        {
            if (value == healthPointMultiplicationModifier)
                return;

            healthPointMultiplicationModifier = value;
            Notify();
        }
    }
    public ActionWrapper healthPointMultiplicationModifierChangeEvent = new();

    [CreateProperty]
    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            if (value == moveSpeed)
                return;

            moveSpeed = value;
            moveSpeedPerFrame = moveSpeed * Time.fixedDeltaTime;
            Notify();
        }
    }
    public ActionWrapper moveSpeedChangeEvent = new();

    [CreateProperty]
    public float DefaultMoveSpeed
    {
        get => defaultMoveSpeed;
        set
        {
            if (value == defaultMoveSpeed)
                return;

            defaultMoveSpeed = value;
            MoveSpeed = defaultMoveSpeed;
            Notify();
        }
    }
    public ActionWrapper defaultMoveSpeedChangeEvent = new();

    [CreateProperty]
    public float ActionMoveSpeedReduceRate
    {
        get => actionMoveSpeedReduceRate;
        set
        {
            if (value == actionMoveSpeedReduceRate)
                return;

            actionMoveSpeedReduceRate = value;
            Notify();
        }
    }
    public ActionWrapper actionMoveSpeedReduceRateChangeEvent = new();

    [CreateProperty]
    public float Size
    {
        get => size;
        set
        {
            if (value == size)
                return;

            size = value;
            Notify();
        }
    }
    public ActionWrapper sizeChangeEvent = new();

    [CreateProperty]
    public float Might
    {
        get => might;
        set
        {
            if (value == might)
                return;

            might = value;
            Notify();
        }
    }
    public ActionWrapper mightChangeEvent = new();

    [CreateProperty]
    public float BaseMight
    {
        get => baseMight;
        set
        {
            if (value == baseMight)
                return;

            baseMight = value;
            Notify();
        }
    }
    public ActionWrapper baseMightChangeEvent = new();

    [CreateProperty]
    public float MightAdditionModifier
    {
        get => mightAdditionModifier;
        set
        {
            if (value == mightAdditionModifier)
                return;

            mightAdditionModifier = value;
            Notify();
        }
    }
    public ActionWrapper mightAdditionModifierChangeEvent = new();

    [CreateProperty]
    public float MightMultiplicationModifier
    {
        get => mightMultiplicationModifier;
        set
        {
            if (value == mightMultiplicationModifier)
                return;

            mightMultiplicationModifier = value;
            Notify();
        }
    }
    public ActionWrapper mightMultiplicationModifierChangeEvent = new();

    [CreateProperty]
    public float Reflex
    {
        get => reflex;
        set
        {
            if (value == reflex)
                return;

            reflex = value;
            Notify();
        }
    }
    public ActionWrapper reflexChangeEvent = new();

    [CreateProperty]
    public float BaseReflex
    {
        get => baseReflex;
        set
        {
            if (value == baseReflex)
                return;

            baseReflex = value;
            Notify();
        }
    }
    public ActionWrapper baseReflexChangeEvent = new();

    [CreateProperty]
    public float ReflexAdditionModifier
    {
        get => reflexAdditionModifier;
        set
        {
            if (value == reflexAdditionModifier)
                return;

            reflexAdditionModifier = value;
            Notify();
        }
    }
    public ActionWrapper reflexAdditionModifierChangeEvent = new();

    [CreateProperty]
    public float ReflexMultiplicationModifier
    {
        get => reflexMultiplicationModifier;
        set
        {
            if (value == reflexMultiplicationModifier)
                return;

            reflexMultiplicationModifier = value;
            Notify();
        }
    }
    public ActionWrapper reflexMultiplicationModifierChangeEvent = new();

    [CreateProperty]
    public float Wisdom
    {
        get => wisdom;
        set
        {
            if (value == wisdom)
                return;

            wisdom = value;
            Notify();
        }
    }
    public ActionWrapper wisdomChangeEvent = new();

    [CreateProperty]
    public float BaseWisdom
    {
        get => baseWisdom;
        set
        {
            if (value == baseWisdom)
                return;

            baseWisdom = value;
            Notify();
        }
    }
    public ActionWrapper baseWisdomChangeEvent = new();

    [CreateProperty]
    public float WisdomAdditionModifier
    {
        get => wisdomAdditionModifier;
        set
        {
            if (value == wisdomAdditionModifier)
                return;

            wisdomAdditionModifier = value;
            Notify();
        }
    }
    public ActionWrapper wisdomAdditionModifierChangeEvent = new();

    [CreateProperty]
    public float WisdomMultiplicationModifier
    {
        get => wisdomMultiplicationModifier;
        set
        {
            if (value == wisdomMultiplicationModifier)
                return;

            wisdomMultiplicationModifier = value;
            Notify();
        }
    }
    public ActionWrapper wisdomMultiplicationModifierChangeEvent = new();

    [CreateProperty]
    public float CurrentManaPoint
    {
        get => currentManaPoint;
        set
        {
            if (value == currentManaPoint)
                return;

            if (value > ManaPoint)
                currentManaPoint = ManaPoint;
            else
                currentManaPoint = value;
            Notify();
        }
    }
    public ActionWrapper currentManaPointChangeEvent = new();

    [CreateProperty]
    public float ManaPoint
    {
        get => manaPoint;
        set
        {
            if (value == manaPoint)
                return;

            manaPoint = value;
            Notify();
        }
    }
    public ActionWrapper manaPointChangeEvent = new();

    [CreateProperty]
    public float BaseManaPoint
    {
        get => baseManaPoint;
        set
        {
            if (value == baseManaPoint)
                return;

            baseManaPoint = value;
            Notify();
        }
    }
    public ActionWrapper baseManaPointChangeEvent = new();

    [CreateProperty]
    public float ManaPointAdditionModifier
    {
        get => manaPointAdditionModifier;
        set
        {
            if (value == manaPointAdditionModifier)
                return;

            manaPointAdditionModifier = value;
            Notify();
        }
    }
    public ActionWrapper manaPointAdditionModifierChangeEvent = new();

    [CreateProperty]
    public float ManaPointMultiplicationModifier
    {
        get => manaPointMultiplicationModifier;
        set
        {
            if (value == manaPointMultiplicationModifier)
                return;

            manaPointMultiplicationModifier = value;
            Notify();
        }
    }
    public ActionWrapper manaPointMultiplicationModifierChangeEvent = new();
    public float manaRegen;
    public float baseManaRegen;
    public float manaRegenAdditionModifier;
    public float manaRegenMultiplicationModifier;

    [CreateProperty]
    public float ManaRegen
    {
        get => manaRegen;
        set
        {
            if (value == manaRegen)
                return;

            manaRegen = value;
            Notify();
        }
    }

    [CreateProperty]
    public float BaseManaRegen
    {
        get => baseManaRegen;
        set
        {
            if (value == baseManaRegen)
                return;

            baseManaRegen = value;
            Notify();
            RecalculateManaRegen();
        }
    }

    [CreateProperty]
    public float ManaRegenAdditionModifier
    {
        get => manaRegenAdditionModifier;
        set
        {
            if (value == manaRegenAdditionModifier)
                return;

            manaRegenAdditionModifier = value;
            Notify();
            RecalculateManaRegen();
        }
    }

    [CreateProperty]
    public float ManaRegenMultiplicationModifier
    {
        get => manaRegenMultiplicationModifier;
        set
        {
            if (value == manaRegenMultiplicationModifier)
                return;

            manaRegenMultiplicationModifier = value;
            Notify();
            RecalculateManaRegen();
        }
    }

    public ActionWrapper manaRegenChangeEvent = new();

    public Dictionary<string, ActionWrapper> propertyChangeEventDictionary = new();
    public ActionWrapper notifyAW;

#if false
    class StatAllInOne{
        float final;
        float base;
        float additionModifier;
        float multiplicationModifier;

        public float Base
        {
            get => base;
            set
            {
                if (value == base)
                    return;

                base = value;
                Notify();
            }
        }

        public float Final
        {
            get => final;
            set
            {
                if (value == final)
                    return;

                final = value;
                finalChange();
            }
        }

        Action recalculateFinal;
        Action finalChange;
    }
#endif
}
