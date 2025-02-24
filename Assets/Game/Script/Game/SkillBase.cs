using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SkillBase : BaseAction
{
	public float duration;
	public float maxRange;
	public int maxAmmo, currentAmmo;
	public Vector3 effectActiveLocation = new Vector3(0, 999, 0);
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}
}