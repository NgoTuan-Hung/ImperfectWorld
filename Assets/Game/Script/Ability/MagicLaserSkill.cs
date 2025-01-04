using UnityEngine;

public class MagicLaserSkill : SkillBase
{
	static ObjectPool magicLaserPool;
	GameObject magicLaserPrefab = Resources.Load("MagicLaser") as GameObject;

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}
	
	
}