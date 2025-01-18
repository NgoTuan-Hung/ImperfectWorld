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
	Slider healthSlider, attackSpeedSlider, defaultMoveSpeedSlider, actionMoveSpeedReduceRateSlider, defaultMagickaSlider, sizeSlider;
	Label healthLabel, attackSpeedLabel, moveSpeedLabel, defaultMoveSpeedLabel, actionMoveSpeedReduceRateLabel, magickaLabel, defaultMagickaLabel, sizeLabel;
	FloatField defaultHealthFloatField;
	
	private void OnEnable() 
	{
		stat = (Stat)target;
		root = statInspectorAsset.Instantiate();
		healthProgress = root.Q<ProgressBar>("inspector__health-progress-bar");
		healthSlider = root.Q<Slider>("inspector__health-slider");
		healthLabel = root.Q<Label>("inspector__health-label");
		defaultHealthFloatField = root.Q<FloatField>("inspector__default-health-float-field");
		attackSpeedSlider = root.Q<Slider>(name: "inspector__attack-speed-slider");
		attackSpeedLabel = root.Q<Label>(name: "inspector__attack-speed-label");
		moveSpeedLabel = root.Q<Label>(name: "inspector__move-speed-label");
		defaultMoveSpeedSlider = root.Q<Slider>(name: "inspector__default-move-speed-slider");
		defaultMoveSpeedLabel = root.Q<Label>(name: "inspector__default-move-speed-label");
		actionMoveSpeedReduceRateSlider = root.Q<Slider>(name: "inspector__action-move-speed-reduce-rate-slider");
		actionMoveSpeedReduceRateLabel = root.Q<Label>(name: "inspector__action-move-speed-reduce-rate-label");
		magickaLabel = root.Q<Label>(name: "inspector__magicka-label");
		defaultMagickaSlider = root.Q<Slider>(name: "inspector__default-magicka-slider");
		defaultMagickaLabel = root.Q<Label>(name: "inspector__default-magicka-label");
		sizeSlider = root.Q<Slider>(name: "inspector__size-slider");
		sizeLabel = root.Q<Label>(name: "inspector__size-label");
		
		healthProgress.lowValue = healthSlider.lowValue = defaultMoveSpeedSlider.lowValue = actionMoveSpeedReduceRateSlider.lowValue
		= defaultMagickaSlider.lowValue = sizeSlider.lowValue = 0;
		attackSpeedSlider.lowValue = 0.01f;
		sizeSlider.highValue = 5f;
		healthSlider.highValue =attackSpeedSlider.highValue = defaultMoveSpeedSlider.highValue = defaultMagickaSlider.highValue = 100;
		actionMoveSpeedReduceRateSlider.highValue = 10f;
		
		root.dataSource = stat;
		healthProgress.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Health)),
			bindingMode = BindingMode.ToTarget
		});
		
		healthProgress.SetBinding("highValue", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultHealth)),
			bindingMode = BindingMode.ToTarget
		});
		
		healthSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Health)),
			bindingMode = BindingMode.TwoWay
		});
		
		healthSlider.SetBinding("highValue", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultHealth)),
			bindingMode = BindingMode.ToTarget
		});
		
		healthLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Health));
		var healthBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		healthBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " 💖";
		});
		healthLabel.SetBinding("text", healthBinding);
		
		defaultHealthFloatField.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultHealth)),
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
		
		actionMoveSpeedReduceRateSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.ActionMoveSpeedReduceRate)),
			bindingMode = BindingMode.TwoWay
		});
		
		actionMoveSpeedReduceRateLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.ActionMoveSpeedReduceRate));
		var actionMoveSpeedReduceRateBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		actionMoveSpeedReduceRateBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " 👠";
		});
		actionMoveSpeedReduceRateLabel.SetBinding("text", actionMoveSpeedReduceRateBinding);
		
		magickaLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Magicka));
		var magickaBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		magickaBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return "Magicka " + value + " 🪄";
		});
		magickaLabel.SetBinding("text", magickaBinding);
		
		defaultMagickaSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultMagicka)),
			bindingMode = BindingMode.TwoWay
		});
		
		defaultMagickaLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.DefaultMagicka));
		var defaultMagickaBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		defaultMagickaBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " 🧙";
		});
		defaultMagickaLabel.SetBinding("text", defaultMagickaBinding);
		
		sizeSlider.SetBinding("value", new DataBinding()
		{
			dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Size)),
			bindingMode = BindingMode.TwoWay
		});
		
		sizeLabel.dataSourcePath = new Unity.Properties.PropertyPath(nameof(Stat.Size));
		var sizeBinding = new DataBinding() {bindingMode = BindingMode.ToTarget};
		sizeBinding.sourceToUiConverters.AddConverter((ref float value) => 
		{
			return value + " ☩";
		});
		sizeLabel.SetBinding("text", sizeBinding);
	}

	public override VisualElement CreateInspectorGUI()
	{
		return root;
	}
	
}

#endif