using UnityEngine;
using UnityEngine.UIElements;

public class Testing : MonoBehaviour 
{
	UIDocument uIDocument;
	RadialProgressUI radialProgressUIOuter, radialProgressUIInner;
	[SerializeField] private PanelSettings panelSettings;
	[SerializeField] private RenderTexture renderTexture;
	[SerializeField] private float progress = 0.1f;
	Material material;
	private void Awake() 
	{
		uIDocument = GetComponent<UIDocument>();
		uIDocument.panelSettings = panelSettings;
		// var tempPanel = Instantiate(panelSettings);
		renderTexture = new RenderTexture(64, 64, 32, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
		renderTexture.Create();
		renderTexture.name = "RenderTexture #" + renderTexture.GetHashCode();
		panelSettings.targetTexture = renderTexture;
		// tempPanel.targetTexture = renderTexture;
		// uIDocument.panelSettings = tempPanel;
		material = GetComponent<MeshRenderer>().material;
		material.mainTexture = renderTexture;
		radialProgressUIOuter = uIDocument.rootVisualElement.Q<RadialProgressUI>(name: "radial-progress-outer");
		radialProgressUIInner = uIDocument.rootVisualElement.Q<RadialProgressUI>(name: "radial-progress-inner");
		var root = radialProgressUIOuter.parent;
		root.style.left = Random.Range(100, 300);
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