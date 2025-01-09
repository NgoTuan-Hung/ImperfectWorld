using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class MovementIntelligence : BaseIntelligence
{	
	Vector2 moveVector;
	public override void Start() 
	{
		base.Start();
		InitAction();
	}
	
	public override void InitAction()
	{	
		AddManuals(customMono.movable.botActionManuals);
	}
}