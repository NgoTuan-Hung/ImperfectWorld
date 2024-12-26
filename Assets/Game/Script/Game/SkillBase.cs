using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SkillBase : BaseAction
{
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}
	
	public virtual void Trigger(Touch touch)
	{
		
	}
	
	public virtual void Trigger()
	{
		
	}
}