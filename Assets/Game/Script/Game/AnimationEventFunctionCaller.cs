using System;
using UnityEngine;

/// <summary>
/// A repository for animation event callbacks. This component must be placed on the
/// same gameobject with the animator component, you will also need to add events manually
/// for each animation.
/// </summary>
public class AnimationEventFunctionCaller : MonoBehaviour 
{
	public bool attack = false;
	public bool endAttack = false;
	public bool castingMagic = false;
	public bool endCastingMagic = false;
	public void Attack() => attack = true;
	public void EndAttack() => endAttack = true;
	public void CastingMagic() => castingMagic = true;
	public void EndCastingMagic() => endCastingMagic = true;
}