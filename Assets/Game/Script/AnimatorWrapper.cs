using UnityEngine;

public class AnimatorWrapper : MonoBehaviour 
{
	private Animator animator;

    public Animator Animator { get => animator; set => animator = value; }

    private void Awake() 
	{
		animator = GetComponentInChildren<Animator>();	
	}
}