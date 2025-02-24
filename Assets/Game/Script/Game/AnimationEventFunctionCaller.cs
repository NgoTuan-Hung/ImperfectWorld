using System;
using UnityEngine;

/// <summary>
/// A repository for animation event callbacks. This component must be placed on the
/// same gameobject with the animator component, you will also need to add events manually
/// for each animation.
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
	public void Attack() => attack = true;
	public void EndAttack() => endAttack = true;
	public void CastingMagic() => castingMagic = true;
	public void EndCastingMagic() => endCastingMagic = true;
	public void Summon() => summon = true;
	public void EndSummon() => endSummon = true;
	public void EndSlaughter() => endSlaughter = true;
	public void EndRelease() => endRelease = true;
	
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
	}
}