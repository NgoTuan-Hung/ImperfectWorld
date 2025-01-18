using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Anything you want to change in the inspector, or to be 
/// affected by other fields, should be placed here. for
/// convinience sake.
/// DON'T REMOVE SERIALIZEFIELD, THEY ARE MEAN TO BE PERSISTED.
/// </summary>
public class Stat : MonoBehaviour, INotifyBindablePropertyChanged
{
	CustomMono customMono;
	[SerializeField] float health = 100f;
	[SerializeField] float defaultHealth = 100f;
	private RadialProgress healthRadialProgress;
	[SerializeField] float attackSpeed = 1;
	[SerializeField] float moveSpeed = 1f;
	[SerializeField] private float defaultMoveSpeed;
	[SerializeField] float actionMoveSpeedReduceRate = 0.1f;
	public float actionMoveSpeedReduced;
	public float moveSpeedPerFrame;
	[SerializeField] float magicka = 1f;
	[SerializeField] float defaultMagicka = 1f;
	[SerializeField] float size = 1f;
	int dieBoolHash = Animator.StringToHash("Die");
	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
	Action onEnable = () => { };

	[CreateProperty]
	public float AttackSpeed 
	{
		get => attackSpeed;
		set
		{
			if (value == attackSpeed) return;
			
			attackSpeed = value;
			Notify();
		}
	}
	public ActionWrapper attackSpeedChangeEvent = new();

	[CreateProperty]
	public float Health { get => health; set 
	{
		if (value == health) return;
		
		health = value;
		if (health <= 0) healthReachZeroEvent.action();
		Notify();
	} }
	public ActionWrapper healthChangeEvent = new();
	public ActionWrapper healthReachZeroEvent = new();
	
	[CreateProperty]
	public float DefaultHealth { get => defaultHealth; set 
	{
		if (value == defaultHealth) return;
		
		defaultHealth = value;
		Health = defaultHealth;
		Notify();
	} }
	public ActionWrapper defaultHealthChangeEvent = new();
	
	[CreateProperty]
	public float MoveSpeed { get => moveSpeed; set 
	{
		if (value == moveSpeed) return;
		
		moveSpeed = value;
		moveSpeedPerFrame = moveSpeed * Time.fixedDeltaTime;
		Notify();
	} }
	public ActionWrapper moveSpeedChangeEvent = new();
	
	[CreateProperty]
	public float DefaultMoveSpeed { get => defaultMoveSpeed; set 
	{
		if (value == defaultMoveSpeed) return;
		
		defaultMoveSpeed = value;
		MoveSpeed = defaultMoveSpeed;
		Notify();
	} }
	public ActionWrapper defaultMoveSpeedChangeEvent = new();
	
	[CreateProperty]
	public float ActionMoveSpeedReduceRate { get => actionMoveSpeedReduceRate; set 
	{
		if (value == actionMoveSpeedReduceRate) return;
		
		actionMoveSpeedReduceRate = value;
		Notify();
	} }
	public ActionWrapper actionMoveSpeedReduceRateChangeEvent = new();
	
	[CreateProperty]
	public float Magicka { get => magicka; set 
	{
		if (value == magicka) return;
		
		magicka = value;
		Notify();
	} }
	public ActionWrapper magickaChangeEvent = new();
	
	[CreateProperty]
	public float DefaultMagicka { get => defaultMagicka; set 
	{
		if (value == defaultMagicka) return;
		
		defaultMagicka = value;
		Magicka = defaultMagicka;
		Notify();
	} }
	public ActionWrapper defaultMagickaChangeEvent = new();
	
	[CreateProperty]
	public float Size { get => size; set 
	{
		if (value == size) return;
		
		size = value;
		Notify();
	} }
	public ActionWrapper sizeChangeEvent = new();
	public Dictionary<string, ActionWrapper> propertyChangeEventDictionary = new();
	
	private void Awake() 
	{
		customMono = GetComponent<CustomMono>();
		AddPropertyChangeEvent();
	}
	
	private void OnEnable() 
	{
		MoveSpeed = DefaultMoveSpeed;
		Health = DefaultHealth;
		onEnable();
	}
	
	private void OnDisable() 
	{
		if (healthRadialProgress != null) healthRadialProgress.transform.parent.gameObject.SetActive(false);
	}
	
	private void Start() 
	{
		InitUI();
		StartCoroutine(LateStart());
		onEnable += () => 
		{
			InitUI();
			InitProperty();
		};
	}
	IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		InitProperty();
	}
	
	void InitUI()
	{
		healthRadialProgress = GameUIManager.Instance.CreateAndHandleRadialProgressFollowing(transform);
	}
	
	public void InitProperty()
	{
		Notify("AttackSpeed");
		Notify("Health");
		Notify("DefaultHealth");
		Notify("MoveSpeed");
		Notify("DefaultMoveSpeed");
		Notify("ActionMoveSpeedReduceRate");
		Notify("Magicka");
		Notify("DefaultMagicka");
		Notify("Size");
	}
	
	void AddPropertyChangeEvent()
	{
		healthChangeEvent.action += () => healthRadialProgress.SetProgress(health / defaultHealth);
		actionMoveSpeedReduceRateChangeEvent.action += () => actionMoveSpeedReduced = defaultMoveSpeed * actionMoveSpeedReduceRate;
		healthReachZeroEvent.action += () => 
		{
			customMono.AnimatorWrapper.SetBool(dieBoolHash, true);
			customMono.combatCollision.SetActive(false);
			healthRadialProgress.transform.parent.gameObject.SetActive(false);
		};
		
		propertyChangeEventDictionary.Add("AttackSpeed", attackSpeedChangeEvent);
		propertyChangeEventDictionary.Add("Health", healthChangeEvent);
		propertyChangeEventDictionary.Add("DefaultHealth", defaultHealthChangeEvent);
		propertyChangeEventDictionary.Add("MoveSpeed", moveSpeedChangeEvent);
		propertyChangeEventDictionary.Add("DefaultMoveSpeed", defaultMoveSpeedChangeEvent);
		propertyChangeEventDictionary.Add("ActionMoveSpeedReduceRate", actionMoveSpeedReduceRateChangeEvent);
		propertyChangeEventDictionary.Add("Magicka", magickaChangeEvent);
		propertyChangeEventDictionary.Add("DefaultMagicka", defaultMagickaChangeEvent);
		propertyChangeEventDictionary.Add("Size", sizeChangeEvent);
	}
	
	void Notify([CallerMemberName] string property = "")
	{
		propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
		if (propertyChangeEventDictionary.ContainsKey(property)) propertyChangeEventDictionary[property].action();
	}
	
	public void SetDefaultMoveSpeed() => MoveSpeed = DefaultMoveSpeed;
}