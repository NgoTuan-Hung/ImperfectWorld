public class ActionIntelligence : BaseIntelligence
{	
	public override void InitAction()
	{
		AddManuals(customMono.attackable.botActionManuals);
	}
	
	public override void Awake() 
	{
		base.Awake();
	}
	
	public override void Start() 
	{
		base.Start();
		InitAction();
	}
}