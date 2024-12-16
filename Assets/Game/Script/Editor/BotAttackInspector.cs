using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(BotAttack))]
public class BotAttackInspector : Editor
{	
	BotAttack botAttack;
	[SerializeField] private VisualTreeAsset botAttackVisualTreeAsset;
	VisualElement root; FloatField floatField, colliderForceFloatField;
	private void OnEnable() 
	{
		botAttack = (BotAttack)target;
		root = botAttackVisualTreeAsset.Instantiate();
		floatField = root.Q<FloatField>("bot-attack-inspector__float-field");
		colliderForceFloatField = root.Q<FloatField>("bot-attack-inspector__collider-force-float-field");
		
		floatField.value = botAttack.MoveSpeedReduceRate;
		botAttack.changeMoveSpeedReduceRate = () => floatField.value = botAttack.MoveSpeedReduceRate;
		floatField.RegisterValueChangedCallback((evt) => 
		{
			botAttack.MoveSpeedReduceRate = floatField.value;
		});
		
		colliderForceFloatField.RegisterValueChangedCallback((evt) => botAttack.ColliderForce = colliderForceFloatField.value);
	}

	public override VisualElement CreateInspectorGUI()
	{
		return root;
	}
	
}
#endif