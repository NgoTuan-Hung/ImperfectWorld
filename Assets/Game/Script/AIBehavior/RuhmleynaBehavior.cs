public class RuhmleynaBehavior : BaseAIBehavior
{
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
        //
    }
}
