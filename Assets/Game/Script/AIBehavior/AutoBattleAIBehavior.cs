using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class AutoBattleAIBehavior : BaseAIBehavior
{
    Attack attack;
    SkillBase skill;
    Stopwatch stopwatch = new();

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        StartCoroutine(LateStart());
        pausableScript.pauseFixedUpdate += StopMove;
    }

    IEnumerator LateStart()
    {
        yield return null;
        attack = GetComponent<Attack>();
        if (customMono.skill.skillBases.Count > 1)
        {
            skill = customMono.skill.skillBases[1];
        }
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

                if (!attack.onCooldown)
                    attack.Trigger(
                        p_direction: customMono.botSensor.centerToTargetCenterDirection,
                        p_customMono: customMono.botSensor.currentNearestEnemy
                    );
            }
        }
    }

    void UseSkill()
    {
        if (skill != null)
        {
            if (
                customMono.stat.currentManaPoint.Value >= customMono.stat.manaPoint.FinalValue
                && customMono.botSensor.distanceToNearestEnemy
                    < skill.GetActionField<ActionFloatField>(ActionFieldName.Range).value
            )
            {
                skill.botActionManual.botDoAction(GetDAPI());
                if (skill.botActionManual.requireContinuous)
                    StartCoroutine(DoActionContinous(skill.botActionManual));
            }
        }
    }

    IEnumerator DoActionContinous(BotActionManual p_botActionManual)
    {
        while (stopwatch.Elapsed.TotalSeconds < p_botActionManual.continousDuration)
        {
            p_botActionManual.botDoActionContinuous(GetDAPI());
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    DoActionParamInfo GetDAPI() => customMono.botSensor.GetDoActionParamInfo();

    void StopMove()
    {
        customMono.movable.StopMove();
    }
}
