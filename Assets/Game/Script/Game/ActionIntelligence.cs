public class ActionIntelligence : BaseIntelligence
{
    public override void InitAction() { }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        InitAction();
        AddManuals(new() { new(ActionUse.Roam, (p_doActionParamInfo) => { }, new()) });
    }
}
