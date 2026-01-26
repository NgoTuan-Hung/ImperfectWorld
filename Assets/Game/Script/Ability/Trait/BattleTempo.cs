public class BattleTempo : SkillBase
{
    AttackGameEventData attackGameEventData = new();

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
        GameManager.Instance.GetSelfEvent(customMono, GameEventType.Attack).action += Proc;
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

    void Proc(IGameEventData p_gED)
    {
        attackGameEventData = p_gED.As<AttackGameEventData>();

        if (attackGameEventData.count % 4 == 0)
        {
            GameManager.Instance.ResolveDamage(
                customMono,
                attackGameEventData.target,
                GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );
        }
    }
}
