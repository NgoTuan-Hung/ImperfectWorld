using System;
using UnityEngine;

public class AnimationEventFunctionCaller : MonoBehaviour 
{
	public Action resetAttackAction;
	public void ResetAttack()
	{
		resetAttackAction?.Invoke();
	}
}