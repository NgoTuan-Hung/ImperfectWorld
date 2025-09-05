using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public float cooldown;
    public bool onCooldown;
    public float defaultCooldown;
    public float defaultStateSpeed;

    /// <summary>
    /// this action should only be executed when
    /// we can use it, and we can use it again
    /// after some cooldown.
    /// </summary>
    public bool canUse;
    public List<BotActionManual> botActionManuals = new();
    public int boolHash = 0;

    /// <summary>
    /// Executing custom callback when animation end.
    /// </summary>
    public Action endAnimCallback = () => { };
    public AnimationClip actionClip;
    public AudioClip audioClip;
    public float damage = 0;
    public float defaultDamage = 0;
    public int maxAmmo,
        currentAmmo;
    public float modifiedAngle;
    public float lifeStealPercent;
    public float stunDuration;
    public Stopwatch stopwatch = new();
    public IEnumerator actionIE,
        botIE;
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
        /* Test */
        if (
            this is ArcaneSwarm
            || this is HeliosGaze
            || this is RimuruCombo2
            || this is RimuruCombo1
            || this is BladeOfMinhKhai
            || this is BladeOfPhong
            || this is BladeOfVu
            || this is DashSkill
            || this is DeepBlade
            || this is DoubleKill
            || this is GetOverThere
            || this is InfernalTide
            || this is LightingForward
            || this is SovereignFlow
            || this is RimuruSummonFireball
        )
        {
            LoadActionFields();
            Config();
        }
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
        yield return new WaitForSecondsRealtime(cooldown);
        onCooldown = false;
        canUse = true;
    }

    /// <summary>
    /// Wait for the animation to end by checking the signal.
    /// You will want to check AnimationEventFunctionCaller
    /// for this.
    /// </summary>
    /// <param name="endAnimCheck"></param>
    public virtual void EndAnimWait(Func<bool> endAnimCheck)
    {
        StartCoroutine(EndAnimWaitCoroutine(endAnimCheck));
    }

    IEnumerator EndAnimWaitCoroutine(Func<bool> endAnimCheck)
    {
        while (!endAnimCheck())
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        onCooldown = true;
        ToggleAnim(boolHash, false);
        endAnimCallback();
        yield return new WaitForSeconds(cooldown);
        canUse = true;
        onCooldown = false;
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

    public virtual ActionResult Trigger(Vector2 p_location = default, Vector2 p_direction = default)
    {
        return failResult;
    }

    public virtual ActionResult StartAndWait()
    {
        return failResult;
    }

    public virtual void WhileWaiting(
        Vector2 p_location = default,
        Vector2 p_direction = default
    ) { }

    public virtual void DoAuto(DoActionParamInfo p_doActionParamInfo) { }

    public virtual void AddAmmo(int ammount) => currentAmmo += ammount;

    public virtual void LifeSteal(float damageDealt) { }

    public virtual void ActionInterrupt()
    {
        customMono.currentAction = null;
    }

    public virtual void Unlock()
    {
        AddActionManuals();
    }
}
