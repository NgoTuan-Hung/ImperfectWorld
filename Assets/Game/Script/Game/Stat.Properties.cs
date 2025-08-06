using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public partial class Stat
{
    CustomMono customMono;
    private PoolObject healthBar;
    public FloatStatWithModifier attackSpeed = new();

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

    public FloatStatWithCap currentHealthPoint = new();
    public Action currentHealthPointReachZeroEvent = () => { };
    public FloatStatWithModifier healthPoint = new();
    public FloatStatWithModifier might = new();
    public FloatStatWithModifier reflex = new();
    public FloatStatWithModifier wisdom = new();
    public FloatStatWithCap currentManaPoint;
    public FloatStatWithModifier manaPoint = new();
    public FloatStatWithModifier manaRegen = new();
    public FloatStatWithModifier healthRegen = new();

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
    public Dictionary<string, ActionWrapper> propertyChangeEventDictionary = new();
    public ActionWrapper notifyAW;
}
