using System.Collections;

public class NewAIBehavior : BaseAIBehavior
{
    AttackRevamp attackRevamp;

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
        attackRevamp = GetComponent<AttackRevamp>();
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
            > attackRevamp.GetActionField<ActionFloatField>(ActionFieldName.Range).value
        )
        {
            customMono.movable.Move(customMono.botSensor.originToTargetOriginDirection);
        }
        else
        {
            customMono.movable.StopMove();

            if (customMono.botSensor.currentNearestEnemy != null)
            {
                if (!attackRevamp.onCooldown)
                    attackRevamp.Trigger(
                        p_direction: customMono.botSensor.centerToTargetCenterDirection,
                        p_customMono: customMono.botSensor.currentNearestEnemy
                    );
            }
        }
    }
}
