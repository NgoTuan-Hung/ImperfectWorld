#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
[CustomEditor(typeof(Stat))]
public class StatInspector : Editor
{
	Stat stat;
	[SerializeField] private VisualTreeAsset statInspectorAsset;
	VisualElement root;
	ProgressBar healthProgress;
	Slider healthSlider;
	
	private void OnEnable() 
	{
		stat = (Stat)target;
		root = statInspectorAsset.Instantiate();
		healthProgress = root.Q<ProgressBar>("inspector__health-progress-bar");
		healthSlider = root.Q<Slider>("inspector__health-slider");
		healthProgress.lowValue = healthSlider.lowValue = 0;
		healthProgress.highValue = healthSlider.highValue = stat.MaxHealth;
		healthSlider.RegisterValueChangedCallback<float>(evt => 
		{
			stat.Health = healthSlider.value;
			healthProgress.value = stat.Health;
		});
	}

	public override VisualElement CreateInspectorGUI()
	{
		return root;
	}
	
}

#endif