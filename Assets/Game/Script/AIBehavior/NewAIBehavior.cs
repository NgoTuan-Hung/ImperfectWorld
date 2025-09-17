using System.Collections;

public class NewAIBehavior : BaseAIBehavior
{
    Attack attack;

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
            customMono.botSensor.centerToTargetCenterDirection.magnitude
            > attack.GetActionField<ActionFloatField>(ActionFieldName.Range).value
        )
        {
            customMono.movable.Move(customMono.botSensor.originToTargetOriginDirection);
        }
        else
        {
            customMono.movable.StopMove();

            if (customMono.botSensor.currentNearestEnemy != null)
            {
                if (!attack.onCooldown)
                    attack.Trigger(
                        p_direction: customMono.botSensor.centerToTargetCenterDirection,
                        p_customMono: customMono.botSensor.currentNearestEnemy
                    );
            }
        }
    }
}
