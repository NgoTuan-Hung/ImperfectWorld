
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
/// </summary>
public class Stat : MonoBehaviour, INotifyBindablePropertyChanged
{
	[SerializeField, HideInInspector] float health = 100f;
	[SerializeField, HideInInspector] private float maxHealth = 100f;
	private RadialProgress healthRadialProgress;
	public float MaxHealth { get => maxHealth; set => maxHealth = value; }
	[SerializeField, HideInInspector] float attackSpeed = 1;
	[SerializeField, HideInInspector] float moveSpeed = 1f;
	[SerializeField, HideInInspector] private float defaultMoveSpeed;
	[SerializeField, HideInInspector] float attackMoveSpeedReduceRate = 0.1f;
	public float attackMoveSpeedReduced;
	public float moveSpeedPerFrame;
	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

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
		Notify();
	} }
	public ActionWrapper healthChangeEvent = new();
	
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
	public float AttackMoveSpeedReduceRate { get => attackMoveSpeedReduceRate; set 
	{
		if (value == attackMoveSpeedReduceRate) return;
		
		attackMoveSpeedReduceRate = value;
		Notify();
	} }
	public ActionWrapper attackMoveSpeedReduceRateChangeEvent = new();
	public Dictionary<string, ActionWrapper> propertyChangeEventDictionary = new();
	
	private void Start() 
	{
		AddPropertyChangeEvent();	
		InitUI();	
	}
	
	void InitUI()
	{
		healthRadialProgress = GameUIManager.Instance.CreateAndHandleRadialProgressFollowing(transform);
	}
	
	public IEnumerator InitProperty()
	{
		yield return new WaitForEndOfFrame();
		
	}
	
	void AddPropertyChangeEvent()
	{
		healthChangeEvent.action += () => healthRadialProgress.SetProgress(health / maxHealth);
		attackMoveSpeedReduceRateChangeEvent.action += () => attackMoveSpeedReduced = defaultMoveSpeed * attackMoveSpeedReduceRate;
		
		propertyChangeEventDictionary.Add("AttackSpeed", attackSpeedChangeEvent);
		propertyChangeEventDictionary.Add("Health", healthChangeEvent);
		propertyChangeEventDictionary.Add("MoveSpeed", moveSpeedChangeEvent);
		propertyChangeEventDictionary.Add("DefaultMoveSpeed", defaultMoveSpeedChangeEvent);
		propertyChangeEventDictionary.Add("AttackMoveSpeedReduceRate", attackMoveSpeedReduceRateChangeEvent);
	}
	
	void Notify([CallerMemberName] string property = "")
	{
		propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
		if (propertyChangeEventDictionary.ContainsKey(property)) propertyChangeEventDictionary[property].action();
	}
	
	public void SetDefaultMoveSpeed() => MoveSpeed = DefaultMoveSpeed;
}