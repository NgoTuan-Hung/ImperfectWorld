using UnityEngine;

public class MyBotPersonality : BaseIntelligence
{
	Vector2 moveVector;
	float distanceToTarget;
	public float meleeRange = 1f;
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}
	
	private void FixedUpdate() 
	{
		Think();
		DoAction();
	}
	
	void Think()
	{
		ThinkAboutNumbers();
		ThinkAboutDistanceToTarget();
		customMono.actionIntelligence.AddActionChance((int)MyAction.NoAttack, 1);
		customMono.movementIntelligence.AddActionChance((int)MoveAction.Idle, 1);
	}
	
	void DoAction()
	{
		customMono.movementIntelligence.ExecuteAnyActionThisFrame(moveVector);
		customMono.actionIntelligence.ExecuteAnyActionThisFrame(moveVector);
	}
	
	void ThinkAboutNumbers()
	{
		moveVector = customMono.Target.transform.position - transform.position;
		distanceToTarget = moveVector.magnitude;
	}
	
	void ThinkAboutDistanceToTarget()
	{
		if (distanceToTarget > meleeRange)
		{
			customMono.movementIntelligence.AddActionChance((int)MoveAction.MoveToTarget, 30);
			customMono.movementIntelligence.AddActionChance((int)MoveAction.MoveRandomly, 1);
		}
		else
		{
			customMono.movementIntelligence.AddActionChance((int)MoveAction.MoveRandomly, 5);
			customMono.movementIntelligence.AddActionChance((int)MoveAction.MoveToTarget, 5);
			customMono.actionIntelligence.AddActionChance((int)MyAction.MeleeAttack, 50);
		}
	}
}