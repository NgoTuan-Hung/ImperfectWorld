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
	Slider healthSlider, attackSpeedSlider, defaultMoveSpeedSlider, attackMoveSpeedReduceRateSlider;
	Label attackSpeedLabel, moveSpeedLabel, defaultMoveSpeedLabel, attackMoveSpeedReduceRateLabel;
	
	private void OnEnable() 
	{
		stat = (Stat)target;
		root = statInspectorAsset.Instantiate();
		healthProgress = root.Q<ProgressBar>("inspector__health-progress-bar");
		healthSlider = root.Q<Slider>("inspector__health-slider");
		attackSpeedSlider = root.Q<Slider>(name: "inspector__attack-speed-slider");
		attackSpeedLabel = root.Q<Label>(name: "inspector__attack-speed-label");
		moveSpeedLabel = root.Q<Label>(name: "inspector__move-speed-label");
		defaultMoveSpeedSlider = root.Q<Slider>(name: "inspector__default-move-speed-slider");
		defaultMoveSpeedLabel = root.Q<Label>(name: "inspector__default-move-speed-label");
		attackMoveSpeedReduceRateSlider = root.Q<Slider>(name: "inspector__attack-move-speed-reduce-rate-slider");
		attackMoveSpeedReduceRateLabel = root.Q<Label>(name: "inspector__attack-move-speed-reduce-rate-label");
		
		healthProgress.lowValue = healthSlider.lowValue = defaultMoveSpeedSlider.lowValue = attackMoveSpeedReduceRateSlider.lowValue = 0;
		attackSpeedSlider.lowValue = 0.01f;
		healthProgress.highValue = healthSlider.highValue = attackSpeedSlider.highValue
		= defaultMoveSpeedSlider.highValue = stat.MaxHealth;
		attackMoveSpeedReduceRateSlider.highValue = 10f;
		
		root.dataSource = stat;
		healthProgress.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Health)),
			bindingMode = BindingMode.ToTarget
		});
		
		healthSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Health)),
			bindingMode = BindingMode.TwoWay
		});
		
		attackSpeedSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.AttackSpeed)),
			bindingMode = BindingMode.TwoWay
		});
		
		attackSpeedLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.AttackSpeed));
		var attackSpeedBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		attackSpeedBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " ⚔";
		});
		attackSpeedLabel.SetBinding("text", attackSpeedBinding);
		
		moveSpeedLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.MoveSpeed));
		var moveSpeedBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		moveSpeedBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return "Move Speed " + value + " 👟";
		});
		moveSpeedLabel.SetBinding("text", moveSpeedBinding);
		
		healthSlider.value = stat.Health;
		
		defaultMoveSpeedSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultMoveSpeed)),
			bindingMode = BindingMode.TwoWay
		});
		
		defaultMoveSpeedLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultMoveSpeed));
		var defaultMoveSpeedBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		defaultMoveSpeedBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " 👢";
		});
		defaultMoveSpeedLabel.SetBinding("text", defaultMoveSpeedBinding);
		
		attackMoveSpeedReduceRateSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.AttackMoveSpeedReduceRate)),
			bindingMode = BindingMode.TwoWay
		});
		
		attackMoveSpeedReduceRateLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.AttackMoveSpeedReduceRate));
		var attackMoveSpeedReduceRateBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		attackMoveSpeedReduceRateBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " 👠";
		});
		attackMoveSpeedReduceRateLabel.SetBinding("text", attackMoveSpeedReduceRateBinding);
	}

	public override VisualElement CreateInspectorGUI()
	{
		return root;
	}
	
}

#endif