using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TheCallOfThePack : SkillBase
{
    GameObject smallWereWolfPrefab;
    static ObjectPool smallWereWolfPool;
    float summonX,
        summonY;

    public override void Awake()
    {
        base.Awake();
        boolHash = Animator.StringToHash("Summon");
        cooldown = defaultCooldown = 60f;
        audioClip = Resources.Load<AudioClip>("AudioClip/the-call-of-the-pack");
        maxRange = 20f;

        smallWereWolfPrefab = Resources.Load("SmallWereWolf") as GameObject;
        smallWereWolfPool ??= new ObjectPool(
            smallWereWolfPrefab,
            10,
            new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
        );

        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.SummonShortRange,
                (direction, location, nextActionChoosingIntervalProposal) =>
                    Call(nextActionChoosingIntervalProposal),
                0.5f
            )
        );
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () =>
        {
            smallWereWolfPool = null;
        };
#endif
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = TriggerCoroutine());
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator TriggerCoroutine()
    {
        while (!customMono.animationEventFunctionCaller.summon)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        /* summon 3 were wolves */
        customMono.animationEventFunctionCaller.summon = false;
        customMono.audioSource.PlayOneShot(audioClip);
        List<PoolObject> poolObjects = smallWereWolfPool.Pick(3);

        foreach (PoolObject poolObject in poolObjects)
            StartCoroutine(DelayAirRoll(poolObject));

        while (customMono.animationEventFunctionCaller.endSummon)
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        customMono.stat.SetDefaultMoveSpeed();
        customMono.actionBlocking = false;
        customMono.animationEventFunctionCaller.endSummon = false;
    }

    IEnumerator DelayAirRoll(PoolObject poolObject)
    {
        /* Place the were wolf along the circle around me with the
        radius of maxRange and make them jump to our position with
        a small offset. */
        CustomMono customMono = poolObject.customMono;
        summonX = Random.Range(-maxRange, maxRange);
        summonY = (float)(
            Math.Sqrt(maxRange * maxRange - summonX * summonX)
            * (Random.Range(-1f, 1f) > 0 ? 1 : -1)
        );
        customMono.transform.position = transform.position + new Vector3(summonX, summonY, 0);
        yield return new WaitForSeconds(Random.Range(0, 0.3f));
        customMono.myBotPersonality.ForceUsingAction(
            ActionUse.AirRoll,
            transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)),
            2f
        );
    }

    public void Call(float duration)
    {
        StartCoroutine(CallCoroutine(duration));
    }

    IEnumerator CallCoroutine(float duration)
    {
        customMono.actionInterval = true;
        Trigger();
        yield return new WaitForSeconds(duration);

        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
        customMono.stat.SetDefaultMoveSpeed();
        customMono.animationEventFunctionCaller.summon = false;
        customMono.animationEventFunctionCaller.endSummon = false;
        StopCoroutine(actionIE);
    }
}
