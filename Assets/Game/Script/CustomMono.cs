using UnityEngine;
public class CustomMono : MonoBehaviour
{
	[SerializeField] private bool isBot = true;
	private GameObject target;
	private GameObject mainComponent;
	private AnimatorWrapper animatorWrapper;
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject Target { get => target; set => target = value; }
    public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }

    private void Awake() 
	{
		animatorWrapper = GetComponent<AnimatorWrapper>();	
		if (isBot)
		{
			target = GameObject.Find("Player");
		}
		mainComponent = transform.Find("MainComponent").gameObject;
	}
}