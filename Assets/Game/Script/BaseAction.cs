using UnityEngine;

[RequireComponent(typeof(CustomMono))]
public class BaseAction : MonoBehaviour 
{
	protected CustomMono customMono;
	
	public virtual void Awake() 
	{
		customMono = GetComponent<CustomMono>();
	}	
}