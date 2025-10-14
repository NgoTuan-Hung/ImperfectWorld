public class BattleTempo : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, null);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        (customMono.skill.skillBases[0] as Attack).hitCallback.callback = Proc;
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.reflex.FinalValue * 2.5f;
    }

    void Proc(HitCallback p_hC)
    {
        if (p_hC.count % 4 == 0)
            p_hC.target.statusEffect.GetHit(
                GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );
    }
}
