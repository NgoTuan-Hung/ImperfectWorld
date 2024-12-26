
using UnityEngine;

public class Stat : MonoBehaviour 
{
	[SerializeField] private float health = 100f;
	[SerializeField] private float maxHealth = 100f;
	private RadialProgress healthRadialProgress;

	public float Health { get => health; set 
	{
		health = value;
		healthRadialProgress.SetProgress(health / maxHealth);
	} }
	public float MaxHealth { get => maxHealth; set => maxHealth = value; }

	private void Start() 
	{
		InitUI();	
	}
	
	void InitUI()
	{
		healthRadialProgress = GameUIManager.Instance.CreateAndHandleRadialProgressFollowing(transform);
	}
}