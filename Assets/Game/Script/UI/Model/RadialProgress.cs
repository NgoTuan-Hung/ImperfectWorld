
using UnityEngine;
using UnityEngine.UIElements;

public class RadialProgress : MonoBehaviour 
{
	UIDocument uIDocument;
	RadialProgressUI radialProgressUIInner;
	RenderTexture renderTexture;
	[SerializeField] private float progress = 0.1f;
	Material material;
	private void OnEnable() 
	{
		uIDocument = GetComponent<UIDocument>();
		
		/*  */
		uIDocument.panelSettings = Instantiate(Resources.Load<PanelSettings>("RadialProgress"));
		renderTexture = Resources.Load<RenderTexture>("RadialProgressRenderTexture");
        renderTexture = new RenderTexture(renderTexture)
        {
            name = "RenderTexture #" + gameObject.name
        };
        uIDocument.panelSettings.targetTexture = renderTexture;
		material = GetComponent<MeshRenderer>().material;
		material.mainTexture = renderTexture;
		/*  */
		
		radialProgressUIInner = uIDocument.rootVisualElement.Q<RadialProgressUI>(name: "radial-progress-inner");	
	}
	
	[SerializeField] private bool change = false;
	private void Update() 
	{
		if (change)
		{
			change = false;
			SetProgress(progress);
		}
	}
	
	public void SetProgress(float progress)
	{
		radialProgressUIInner.Progress = progress;
	}
}