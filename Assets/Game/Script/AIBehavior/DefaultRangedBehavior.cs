public class DefaultRangedBehavior : BaseAIBehavior
{
    public float logicalAttackRange = 7f; //
    public float targetTooCloseRange = 3f;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void DoFixedUpdate()
    {
        ThinkAndPrepare();
        base.DoAction();
    }

    void ThinkAndPrepare()
    {
        /* Here you can add maximum chance for any action you wish to execute,
        however, remember to reset delegate afterward.*/
        forceUsingAction();

        /* If there is no nearest enemy, move to the enemy detected on radar,
        or roam instead. */
        if (customMono.botSensor.currentNearestEnemy == null)
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Roam, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Roam, 50000);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 50000);
        }
        else
            Think();
    }

    void Think()
    {
        customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 1);
        customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 1);
        customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 1);
        customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 1);

        // if (customMono.attackable.onCooldown)
        // {
        // 	customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
        // 	customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
        // 	customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
        // 	customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
        // }

        if (customMono.botSensor.distanceToNearestEnemy > targetTooCloseRange)
        {
            if (customMono.botSensor.distanceToNearestEnemy < logicalAttackRange)
            {
                customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
                customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 1);
                customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 1);
            }
            else
            {
                customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
                customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
                customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
            }
        }
        else
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 10);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 10);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.PushAway, 10);
        }
    }
}
