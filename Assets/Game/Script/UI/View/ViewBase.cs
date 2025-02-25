using UnityEngine;

public class ViewBase : MonoBehaviour
{
	public GameUIManager gameUIManager;
	
	public virtual void Init()
	{
		LoadAllTemplate();
		GetAllRequiredVisualElements();
	}
	
	public virtual void GetAllRequiredVisualElements()
	{
		
	}
	
	public virtual void LoadAllTemplate() {}
	public virtual void InitIndividualView(IndividualView p_individualView, CharUIData p_charUIData) {}
}