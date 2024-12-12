using UnityEngine;

[RequireComponent(typeof(CustomMono))]
public class BaseAction : MonoBehaviour 
{
	protected CustomMono customMono;
	
	public virtual void Awake() 
	{
		customMono = GetComponent<CustomMono>();
	}
	
	public virtual void ToggleAnim(int boolHash, bool value)
	{
		customMono.AnimatorWrapper.Animator.SetBool(boolHash, value);
	}
}