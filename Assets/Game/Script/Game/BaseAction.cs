using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionUse
{
    GetCloser,
    GetAway,
    Dodge,
    Passive,
    MeleeDamage,
    RangedDamage,
    Roam,
    AirRoll,
    SummonShortRange,
    PushAway,
    KeepDistance,
}

[RequireComponent(typeof(CustomMono))]
public partial class BaseAction : MonoEditor
{
    public CustomMono customMono;
    public bool onCooldown;
    public float defaultCooldown;
    public float defaultStateSpeed = 1;

    /// <summary>
    /// this action should only be executed when
    /// we can use it, and we can use it again
    /// after some cooldown.
    /// </summary>
    public bool canUse;
    public AudioClip audioClip;
    public BotActionManual botActionManual;
    public IEnumerator botIE;
    public static ActionResult failResult = new();
    public ActionResult successResult = new(true, ActionResultType.Cooldown, default);
    Dictionary<ActionFieldName, ActionField> actionFields = new();

    public virtual void Awake()
    {
        customMono = GetComponent<CustomMono>();
    }

    public virtual void OnEnable()
    {
        onCooldown = false;
        canUse = true;
    }

    public override void Start()
    {
        base.Start();
        /* Stop action when we die */
        customMono.stat.currentHealthPointReachZeroEvent += StopAndDisable;
        LoadActionFields();
        Config();
    }

    private void LoadActionFields()
    {
        ActionFieldInfo t_aFI = GameManager.Instance.GetActionFieldInfo(GetType().Name);
        t_aFI.actionFieldNames.ForEach(aFN =>
        {
            actionFields.Add(
                aFN,
                Activator.CreateInstance(GameManager.Instance.GetActionFieldTypeFromName(aFN))
                    as ActionField
            );
        });
    }

    public T GetActionField<T>(ActionFieldName p_actionFieldName)
        where T : ActionField => actionFields[p_actionFieldName] as T;

    public virtual void Config() { }

    public virtual void ToggleAnim(int boolHash, bool value)
    {
        customMono.AnimatorWrapper.animator.SetBool(boolHash, value);
    }

    public virtual void SetBlend(int blendHash, float value)
    {
        customMono.AnimatorWrapper.animator.SetFloat(blendHash, value);
    }

    public virtual void AddActionManuals() { }

    public virtual bool GetBool(int boolHash) =>
        customMono.AnimatorWrapper.animator.GetBool(boolHash);

    public IEnumerator CooldownCoroutine()
    {
        onCooldown = true;
        yield return new WaitForSecondsRealtime(
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        onCooldown = false;
        canUse = true;
    }

    public virtual void StatChangeRegister() { }

    /// <summary>
    /// Should be called when any stat change (might change => more damage for skill, etc.)
    /// </summary>
    public virtual void RecalculateStat() { }

    public virtual void StopAndDisable()
    {
        if (customMono.currentAction == this)
        {
            ActionInterrupt();
        }
        // StopAllCoroutines();
    }

    public virtual ActionResult Trigger(
        Vector2 p_location = default,
        Vector2 p_direction = default,
        CustomMono p_customMono = null
    )
    {
        return failResult;
    }

    public virtual ActionResult StartAndWait()
    {
        return failResult;
    }

    public virtual void TriggerContinuous(
        Vector2 p_location = default,
        Vector2 p_direction = default
    ) { }

    public virtual void BotDoAction(DoActionParamInfo p_doActionParamInfo) { }

    public virtual void BotDoActionContinous(DoActionParamInfo p_doActionParamInfo) { }

    public virtual void DoAuto(DoActionParamInfo p_doActionParamInfo) { }

    public virtual void LifeSteal(float damageDealt) { }

    public virtual void ActionInterrupt()
    {
        customMono.currentAction = null;
    }

    /// <summary>
    /// Return final damage value considering all damage modifiers (Buff, Debuff, etc.)
    /// </summary>
    /// <param name="p_damage"></param>
    /// <returns></returns>
    public float CalculateFinalDamage(float p_damage) =>
        customMono.stat.damageModifier.CalculateValueWithAppliedModifiers(p_damage);
}
