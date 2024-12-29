using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CustomMono))]
public class AnimatorWrapper : MonoBehaviour 
{
	public Animator animator;
	public enum AddAnimationEventMode {Start, End, Any}

	private void Awake() 
	{
		animator = GetComponentInChildren<Animator>();	
	}
	
	public AnimationClip GetAnimationClip(string clipName)
	{
		return animator.runtimeAnimatorController.animationClips.First(clip => clip.name.Equals(clipName));
	}
	public void AddAnimationEvent(AnimationClip animationClip, string functionName, AddAnimationEventMode mode, float time = 0)
	{
		AnimationEvent animationEvent = new AnimationEvent();
		switch (mode)
		{
			case AddAnimationEventMode.Start:
				animationEvent.time = 0;
				animationEvent.functionName = functionName;
				break;
			case AddAnimationEventMode.End:
				animationEvent.time = animationClip.length;
				animationEvent.functionName = functionName;
				break;
			case AddAnimationEventMode.Any:
				animationEvent.time = time;
				animationEvent.functionName = functionName;
				break;
		}
		animationClip.AddEvent(animationEvent);
	}
}