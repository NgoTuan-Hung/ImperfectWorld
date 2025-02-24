using System;
using UnityEngine;

/// <summary>
/// A notifier class for changes in monobehaviour.
/// </summary>
public class MonoSelfAware : MonoBehaviour 
{
	public Action deactivate;
	public virtual void Awake()
	{
		deactivate = Deactivate;
	}
	void Deactivate() => gameObject.SetActive(false);
}