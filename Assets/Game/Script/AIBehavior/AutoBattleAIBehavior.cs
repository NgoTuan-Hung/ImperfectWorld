using System;
using System.Collections;
using UnityEngine;

public class AutoBattleAIBehavior : BaseAIBehavior
{
    float timer = 0,
        minUsageRadius;

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        pausableScript.pauseFixedUpdate += StopMove;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void DoFixedUpdate()
    {
        if (customMono.botSensor.currentNearestEnemy == null)
        {
            //
        }
        else
        {
            minUsageRadius =
                customMono.stat.attackRange.FinalValue
                > customMono.mainSkill.GetActionField<ActionFloatField>(ActionFieldName.Range).value
                    ? customMono
                        .mainSkill.GetActionField<ActionFloatField>(ActionFieldName.Range)
                        .value
                    : customMono.stat.attackRange.FinalValue;

            UseSkill();
            Attack();
            if (customMono.botSensor.distanceToNearestEnemy <= minUsageRadius)
                customMono.movable.StopMove();
            else
                customMono.movable.Move(customMono.botSensor.originToTargetOriginDirection);
        }
    }

    void UseSkill()
    {
        if (
            customMono.stat.currentManaPoint.Value >= customMono.stat.manaPoint.FinalValue
            && customMono.botSensor.distanceToNearestEnemy
                < customMono.mainSkill.GetActionField<ActionFloatField>(ActionFieldName.Range).value
        )
        {
            customMono.mainSkill.botActionManual.botDoAction(GetDAPI());
            if (customMono.mainSkill.botActionManual.requireContinuous)
                StartCoroutine(DoActionContinous(customMono.mainSkill.botActionManual));
        }
    }

    void Attack()
    {
        if (
            !customMono.attack.onCooldown
            && customMono.botSensor.distanceToNearestEnemy < customMono.stat.attackRange.FinalValue
        )
            customMono.attack.Trigger(
                p_direction: customMono.botSensor.centerToTargetCenterDirection,
                p_customMono: customMono.botSensor.currentNearestEnemy
            );
    }

    IEnumerator DoActionContinous(BotActionManual p_botActionManual)
    {
        timer = 0;
        while (timer < p_botActionManual.continousDuration)
        {
            p_botActionManual.botDoActionContinuous(GetDAPI());
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
        }
    }

    DoActionParamInfo GetDAPI() => customMono.botSensor.GetDoActionParamInfo();

    void StopMove()
    {
        customMono.movable.StopMove();
    }
}
