using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class NewAIBehavior : BaseAIBehavior
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
        // ThinkAndPrepare();
        // base.DoAction();

        if (
            customMono.botSensor.originToTargetOriginDirection.magnitude
            > attack.GetActionField<ActionFloatField>(ActionFieldName.Range).value
        )
        {
            customMono.movable.Move(customMono.botSensor.targetPathFindingDirection);
        }
        else
        {
            customMono.movable.StopMove();

            if (customMono.botSensor.currentNearestEnemy != null)
            {
                if (skill != null)
                {
                    if (
                        skill.GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
                        <= customMono.stat.currentManaPoint.Value
                    )
                        UseSkill();
                }

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
        skill.botActionManual.botDoAction(GetDAPI());
        if (skill.botActionManual.requireContinuous)
            StartCoroutine(DoActionContinous(skill.botActionManual));
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
}
