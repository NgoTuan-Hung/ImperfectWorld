using System;
using System.Collections;
using UnityEngine;

public class AutoBattleAIBehavior : BaseAIBehavior
{
    float timer = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
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
            UseSkill();

            if (
                customMono.botSensor.originToTargetOriginDirection.magnitude
                > customMono.stat.attackRange.FinalValue
            )
            {
                customMono.movable.Move(customMono.botSensor.targetPathFindingDirection);
            }
            else
            {
                customMono.movable.StopMove();

                if (!customMono.attack.onCooldown)
                    customMono.attack.Trigger(
                        p_direction: customMono.botSensor.centerToTargetCenterDirection,
                        p_customMono: customMono.botSensor.currentNearestEnemy
                    );
            }
        }
    }

    void UseSkill()
    {
        if (customMono.mainSkill != null)
        {
            if (
                customMono.stat.currentManaPoint.Value >= customMono.stat.manaPoint.FinalValue
                && customMono.botSensor.distanceToNearestEnemy
                    < customMono
                        .mainSkill.GetActionField<ActionFloatField>(ActionFieldName.Range)
                        .value
            )
            {
                customMono.mainSkill.botActionManual.botDoAction(GetDAPI());
                if (customMono.mainSkill.botActionManual.requireContinuous)
                    StartCoroutine(DoActionContinous(customMono.mainSkill.botActionManual));
            }
        }
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
