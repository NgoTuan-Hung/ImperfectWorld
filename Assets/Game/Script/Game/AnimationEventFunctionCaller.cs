using System;
using System.Collections;
using UnityEngine;

public enum EAnimationSignal
{
    Attack,
    EndAttack,
    Combo1Signal,
    EndCombo1,
    Combo2Signal,
    EndCombo2,
    Summon,
    EndSummon,
    EndRelease,
    MainSkill1Signal,
    EndMainSkill1,
    MainSkill2Signal,
    EndMainSkill2,
    MainSkill3Signal,
    EndMainSkill3,
    MainSkill4Signal,
    EndMainSkill4,
}

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
    private void OnEnable()
    {
        ResetVals();
    }

    BitArray signalVals;

    public bool GetSignalVals(EAnimationSignal p_eAS) => signalVals.Get((int)p_eAS);

    private void Awake()
    {
        signalVals = new(Enum.GetNames(typeof(EAnimationSignal)).Length);
    }

    public void SetSignal(EAnimationSignal p_eAS) => signalVals[(int)p_eAS] = true;

    public void SetSignal(EAnimationSignal p_eAS, bool p_value) => signalVals[(int)p_eAS] = p_value;

    void ResetVals()
    {
        signalVals.SetAll(false);
    }
}
