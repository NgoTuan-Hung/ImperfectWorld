using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHollowShepherd : SkillBase
{
    HashSet<CustomMono> handledAllies;
    List<FloatStatModifier> modifiers;
    ValueChangeGameEventData hpChangeGED;

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
        /* Stun duration */
        // successResult = new(
        //     true,
        //     ActionResultType.Cooldown,
        //     GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        // );
        /* Debuff duration */

        GameManager
            .Instance.GetTeamBasedEvent(customMono.tag, GameEventType.HPChange)
            .action += Trigger;
        /* Also use actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        // customMono.stat.attackSpeed.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
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
                var t_mod = new FloatStatModifier(3f, FloatStatModifierType.Multiplicative);
                customMono.stat.attackSpeed.AddModifier(t_mod);
                modifiers.Add(t_mod);
                hpChangeGED.owner.statusEffect.GetHit(hpChangeGED.maxValue * 0.1f);
            }
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.GetTeamBasedEvent(customMono.tag, GameEventType.HPChange).action -=
            Trigger;
    }
}
