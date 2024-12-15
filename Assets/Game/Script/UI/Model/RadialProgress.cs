
using UnityEngine;
using UnityEngine.UIElements;

public class RadialProgress : MonoBehaviour 
{
	UIDocument uIDocument;
	RadialProgressUI radialProgressUI;
	private void Awake() 
	{
		uIDocument = GetComponent<UIDocument>();
		radialProgressUI = uIDocument.rootVisualElement.Q<RadialProgressUI>(name: "radial-progress-inner");
	}
	
	public void SetProgress(float progress)
	{
		radialProgressUI.Progress = progress;
	}
}