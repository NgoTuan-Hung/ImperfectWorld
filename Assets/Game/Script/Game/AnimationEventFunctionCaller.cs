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
    public bool castingMagic;
    public bool endCastingMagic;
    public bool summon;
    public bool endSummon;
    public bool endSlaughter;
    public bool endRelease;
    public bool bladeOfMinhKhaiSpawnSlash;
    public bool endBladeOfMinhKhai;
    public bool skill2AnimSignal;
    public bool skill2AnimEnd;

    public void Attack() => attack = true;

    public void EndAttack() => endAttack = true;

    public void CastingMagic() => castingMagic = true;

    public void EndCastingMagic() => endCastingMagic = true;

    public void Summon() => summon = true;

    public void EndSummon() => endSummon = true;

    public void EndSlaughter() => endSlaughter = true;

    public void EndRelease() => endRelease = true;

    public void BladeOfMinhKhaiSpawnSlash() => bladeOfMinhKhaiSpawnSlash = true;

    public void EndBladeOfMinhKhai() => endBladeOfMinhKhai = true;

    public void Skill2AnimSignal() => skill2AnimSignal = true;

    public void Skill2AnimEnd() => skill2AnimEnd = true;

    private void OnEnable()
    {
        attack = false;
        endAttack = false;
        castingMagic = false;
        endCastingMagic = false;
        summon = false;
        endSummon = false;
        endSlaughter = false;
        endRelease = false;
        bladeOfMinhKhaiSpawnSlash = false;
        endBladeOfMinhKhai = false;
        skill2AnimSignal = false;
        skill2AnimEnd = false;
    }
}
