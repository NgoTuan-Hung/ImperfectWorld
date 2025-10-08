// using UnityEngine;
// using UnityEngine.UIElements;

// public class Testing : MonoBehaviour
// {
// 	UIDocument uIDocument;
// 	RadialProgressUI radialProgressUIOuter, radialProgressUIInner;
// 	[SerializeField] private PanelSettings panelSettings;
// 	[SerializeField] private RenderTexture renderTexture;
// 	[SerializeField] private float progress = 0.1f;
// 	Material material;
// 	private void Awake()
// 	{
// 		uIDocument = GetComponent<UIDocument>();

// 		/*  */
// 		uIDocument.panelSettings = Instantiate(Resources.Load<PanelSettings>("RadialProgress"));
// 		renderTexture = new RenderTexture(renderTexture);
// 		renderTexture.name = "RenderTexture #" + gameObject.name;
// 		uIDocument.panelSettings.targetTexture = renderTexture;
// 		material = GetComponent<MeshRenderer>().material;
// 		material.mainTexture = renderTexture;
// 		/*  */

// 		radialProgressUIOuter = uIDocument.rootVisualElement.Q<RadialProgressUI>(name: "radial-progress-outer");
// 		radialProgressUIInner = uIDocument.rootVisualElement.Q<RadialProgressUI>(name: "radial-progress-inner");
// 		// var root = radialProgressUIOuter.parent;
// 		// root.style.left = Random.Range(100, 300);
// 	}

// 	[SerializeField] private bool change = false;
// 	private void Update()
// 	{
// 		if (change)
// 		{
// 			change = false;
// 			SetProgress(progress);
// 		}
// 	}

// 	public void SetProgress(float progress)
// 	{
// 		radialProgressUIInner.Progress = progress;
// 	}
// }
