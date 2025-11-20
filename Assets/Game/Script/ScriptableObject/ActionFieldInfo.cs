using System.Collections.Generic;
using UnityEngine;

public enum ActionFieldName
{
    Cooldown,
    Duration,
    Range,
    Damage,
    Variants,
    ManaCost,
    LifeStealPercent,
    CurrentTime,
    Speed,
    ActionIE,
    ActionIE1,
    ActionIE2,
    EffectCount,
    EffectDuration,
    Interval,
    CustomGameObject,
    PoisonInfo,
    SlowInfo,
    GameEffect,
    Origin,
    StopWatch,
    Angle,
    Direction,
    Blend,
    Target,
    CurrentPhase,
    ComboActions,
    ComboEffects,
    ComboEndAction,
    Distance,
    SelectedVariant,
    AllPhases,
    UseCount,
    MaxUseCount,
}

[CreateAssetMenu(fileName = "ActionFieldInfo", menuName = "ScriptableObjects/ActionFieldInfo")]
public class ActionFieldInfo : ScriptableObject
{
    public string actionType;
    public List<ActionFieldName> actionFieldNames;
}
