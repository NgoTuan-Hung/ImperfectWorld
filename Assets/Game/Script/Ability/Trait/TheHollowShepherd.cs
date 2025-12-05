using System.Collections.Generic;

public class TheHollowShepherd : SkillBase
{
    HashSet<CustomMono> handledAllies;
    List<FloatStatModifier> modifiers;
    ValueChangeGameEventData hpChangeGED;
    float aspdBuff;

    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, null);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        modifiers?.ForEach(m => customMono.stat.attackSpeed.RemoveModifier(m));
        modifiers = new();
        handledAllies = new();
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        GameManager.Instance.GetTeamBasedEvent(customMono.tag, GameEventType.HPChange).action +=
            Trigger;
        /* Also use actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        aspdBuff = 0.5f + customMono.stat.reflex.FinalValue * 0.01f;
    }

    public void Trigger(IGameEventData p_gED)
    {
        hpChangeGED = p_gED.As<ValueChangeGameEventData>();

        if (hpChangeGED.owner.Equals(customMono))
            return;
        if (!handledAllies.Contains(hpChangeGED.owner))
        {
            if (hpChangeGED.currentValue / hpChangeGED.maxValue < 0.2f)
            {
                handledAllies.Add(hpChangeGED.owner);
                var aspdFSM = new FloatStatModifier(aspdBuff, ModifierType.Additive);
                customMono.stat.attackSpeed.AddModifier(aspdFSM);
                modifiers.Add(aspdFSM);
                hpChangeGED.owner.statusEffect.GetHit(customMono, hpChangeGED.maxValue * 0.1f);
            }
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.GetTeamBasedEvent(customMono.tag, GameEventType.HPChange).action -=
            Trigger;
    }
}
