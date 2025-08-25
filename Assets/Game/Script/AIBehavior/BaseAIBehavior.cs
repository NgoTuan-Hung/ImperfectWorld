using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Bot brain, execute actions based on various factors like sensors, conditions,...
/// Should run after BotSensor.cs .
/// </summary>
[DefaultExecutionOrder(0)]
public class BaseAIBehavior : CustomMonoPal
{
    public Action forceUsingAction = () => { };
    public PausableScript pausableScript = new();

    public override void Start()
    {
        base.Start();
        StartCoroutine(LateStart());
    }

    /// <summary>
    /// This script can be paused and resumed, just need to
    /// </summary>
    /// <returns></returns>
    IEnumerator LateStart()
    {
        yield return null;
        pausableScript.resumeFixedUpdate = ResumeFixedUpdate;
        pausableScript.pauseFixedUpdate = PauseFixedUpdate;
        pausableScript.resumeFixedUpdate();
    }

    public virtual void DoFixedUpdate() { }

    public virtual void FixedUpdate()
    {
        pausableScript.fixedUpdate();
    }

    void PauseFixedUpdate() => pausableScript.fixedUpdate = NoFixedUpdate;

    void ResumeFixedUpdate() => pausableScript.fixedUpdate = DoFixedUpdate;

    public static void NoFixedUpdate() { }

    /// <summary>
    /// Select two action this frame, one for main action (affecting Sprite, Animation, ...)
    /// and one for movement action (affecting movement and Sprite with lower priority) and execute
    /// them.
    /// </summary>
    public virtual void DoAction()
    {
        /* IMPORTANT */
        customMono.movementIntelligence.ExecuteTheMostFavorableAction(
            customMono.movementActionInterval,
            p_originToTargetOriginDirection: customMono.botSensor.originToTargetOriginDirection,
            p_centerToTargetCenterDirection: customMono.botSensor.centerToTargetCenterDirection,
            p_firePointToTargetCenterDirection: customMono
                .botSensor
                .firePointToTargetCenterDirection,
            p_targetOriginPosition: customMono.botSensor.targetOriginPosition,
            p_targetCenterPosition: customMono.botSensor.targetCenterPosition
        );
        customMono.actionIntelligence.ExecuteTheMostFavorableAction(
            customMono.actionInterval,
            p_originToTargetOriginDirection: customMono.botSensor.originToTargetOriginDirection,
            p_centerToTargetCenterDirection: customMono.botSensor.centerToTargetCenterDirection,
            p_firePointToTargetCenterDirection: customMono
                .botSensor
                .firePointToTargetCenterDirection,
            p_targetOriginPosition: customMono.botSensor.targetOriginPosition,
            p_targetCenterPosition: customMono.botSensor.targetCenterPosition
        );
    }

    public void ForceUsingAction(ActionUse actionUse, Vector3 targetPositionParam, float duration)
    {
        void t_forceUsingAction()
        {
            customMono.botSensor.SetOriginToTargetOriginDirection(
                customMono.botSensor.currentNearestEnemy.transform.position - transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.botSensor.SetCenterToTargetCenterDirection(
                customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform.position
                    - customMono.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.botSensor.SetTargetOriginPosition(
                targetPositionParam,
                ModificationPriority.VeryHigh
            );
            customMono.botSensor.SetTargetCenterPosition(
                customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.actionIntelligence.PreSumActionChance(actionUse, 9999);
        }

        forceUsingAction += t_forceUsingAction;
        StartCoroutine(ForceUsingActionCoroutine(t_forceUsingAction, duration));
    }

    IEnumerator ForceUsingActionCoroutine(Action action, float duration)
    {
        yield return new WaitForSeconds(duration);

        forceUsingAction -= action;
    }
}
