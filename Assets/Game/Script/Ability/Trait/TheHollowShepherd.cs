using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHollowShepherd : SkillBase
{
    HashSet<CustomMono> handledAllies;
    List<FloatStatModifier> modifiers;

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

    public void Trigger(GameEventData p_gED)
    {
        if (p_gED.owner == customMono)
            return;
        if (!handledAllies.Contains(p_gED.owner))
        {
            if (p_gED.currentValue / p_gED.maxValue < 0.2f)
            {
                var t_mod = new FloatStatModifier(3f, FloatStatModifierType.Multiplicative);
                customMono.stat.attackSpeed.AddModifier(t_mod);
                modifiers.Add(t_mod);
                p_gED.owner.statusEffect.GetHit(p_gED.maxValue * 0.1f);
                handledAllies.Add(p_gED.owner);
            }
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.GetTeamBasedEvent(customMono.tag, GameEventType.HPChange).action -=
            Trigger;
    }
}
