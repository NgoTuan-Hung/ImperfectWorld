using System;
using UnityEngine;

/// <summary>
/// A repository for animation event callbacks. This component must be placed on the
/// same gameobject with the animator component, you will also need to add events manually
/// for each animation.
/// - A note for this is that you can use the same animation for multiple action because
/// action are mutually exclusive in their execution, which mean 2 actions of the same type
/// can't be both active at the same time. You can place animation event in your animation
/// clip and use them in your action by waiting for the signal which is the boolean to be
/// true.
/// </summary>
public class AnimationEventFunctionCaller : MonoBehaviour
{
    public bool attack;
    public bool endAttack;
    public bool combo1Signal;
    public bool endCombo1;
    public bool combo2Signal;
    public bool endCombo2;
    public bool summon;
    public bool endSummon;
    public bool endRelease;
    public AnimationSignal mainSkill1AS = new();
    public bool mainSkill2Signal;
    public bool endMainSkill2;
    public bool mainSkill3Signal;
    public bool endMainSkill3;

    public void Attack() => attack = true;

    public void EndAttack() => endAttack = true;

    public void Combo1Signal() => combo1Signal = true;

    public void EndCombo1() => endCombo1 = true;

    public void Combo2Signal() => combo2Signal = true;

    public void EndCombo2() => endCombo2 = true;

    public void Summon() => summon = true;

    public void EndSummon() => endSummon = true;

    public void EndRelease() => endRelease = true;

    public void MainSkill1Signal() => mainSkill1AS.signal = true;

    public void EndMainSkill1() => mainSkill1AS.end = true;

    public void MainSkill2Signal() => mainSkill2Signal = true;

    public void EndMainSkill2() => endMainSkill2 = true;

    public void MainSkill3Signal() => mainSkill3Signal = true;

    public void EndMainSkill3() => endMainSkill3 = true;

    private void OnEnable()
    {
        attack = false;
        endAttack = false;
        combo1Signal = false;
        endCombo1 = false;
        combo2Signal = false;
        endCombo2 = false;
        summon = false;
        endSummon = false;
        endRelease = false;
        mainSkill1AS.signal = false;
        mainSkill1AS.end = false;
        mainSkill2Signal = false;
        endMainSkill2 = false;
        mainSkill3Signal = false;
        endMainSkill3 = false;
    }
}
