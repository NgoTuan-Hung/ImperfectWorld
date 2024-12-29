
using UnityEngine;
using UnityEngine.UIElements;

public class RadialProgress : MonoBehaviour 
{
	UIDocument uIDocument;
	RadialProgressUI radialProgressUIInner;
	RenderTexture renderTexture;
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
	
	public void SetProgress(float progress)
	{
		radialProgressUIInner.Progress = progress;
	}
}