using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;

[RequireComponent(typeof(CustomMono))]
public class BaseIntelligence : MonoEditor
{
	protected CustomMono customMono;
	public List<int> actionChances = new();
	/// <summary>
	/// Say we have 3 action with action chance 30 60 10
	/// then the normalized values will be 0.3 0.6 0.1
	/// and the cumulative distribution will be 0.3 0.9 1.
	/// </summary>
	public List<float> actionCumulativeDistribution = new();
	public List<BotActionManual> botActionManuals = new();
	public List<List<BotActionManual>> botActionManualCategories = new();
	public List<int> presummedActionChances = new();
	
	public virtual void Awake() 
	{
		customMono = GetComponent<CustomMono>();
		var t_actionUses = Enum.GetNames(typeof(ActionUse));
		for (int i=0;i<t_actionUses.Length;i++)
		{
			botActionManualCategories.Add(new List<BotActionManual>());
			presummedActionChances.Add(0);
		}
	}
	
	public virtual void InitAction(){}
	
	/// <summary>
	/// Normalize action chances and calculate cumulative distribution of 
	/// each action.
	/// </summary>
	void RecalculateActionCumulativeDistribution()
	{
		float t_total = actionChances.Sum();
		
		actionCumulativeDistribution[0] = actionChances[0] / t_total;
		for (int i=1;i<actionCumulativeDistribution.Count;i++)
		{
			actionCumulativeDistribution[i] = actionCumulativeDistribution[i-1] + actionChances[i] / t_total;
		}
	}
	
	/// <summary>
	/// So you have cumulativeDistribution of 0.3 0.9 1, that means 0-0.3 is the distribution of 
	/// action 0 and it has 30% chance, 0.3-0.9 is the distribution of action 1 and it has 60% chance
	/// and so on. To pick a random action, we can search if our random value is less than any
	/// cumulative distribution while we iterate from left to right, or we can do a binary search.
	/// </summary>
	/// <returns></returns>
	public void ExecuteAnyActionThisFrame(bool actionInterval, Vector2 targetDirection = default, Vector2 targetPosition = default)
	{
		if (actionInterval) 
		{
			RefreshActionChances();
			return;
		}
		
		CalculatingFinalActionChances();
		RecalculateActionCumulativeDistribution();
		var rand = Random.Range(0, 1f);
		BotActionManual t_actionManual = botActionManuals[actionCumulativeDistribution.CumulativeDistributionBinarySearch
		(
			0, actionCumulativeDistribution.Count-1, rand
		)];
		
		if (t_actionManual.targetDirection) targetDirection *= t_actionManual.targetDirectionMultiplier;
		t_actionManual.doAction(targetDirection, targetPosition, t_actionManual.nextActionChoosingIntervalProposal);
		
		RefreshActionChances();
	}
	
	/// <summary>
	/// Set all the value to 0
	/// </summary>
	public void RefreshActionChances()
	{
		for (int i = 0; i < actionChances.Count; i++) actionChances[i] = 0;
		for (int i = 0; i < presummedActionChances.Count; i++) presummedActionChances[i] = 0;
	}
	
	/// <summary>
	/// Loop through each category only once and set the final action chance
	/// of each action.
	/// </summary>
	void CalculatingFinalActionChances()
	{
		for (int i=0;i<botActionManualCategories.Count;i++)
			for (int j=0;j<botActionManualCategories[i].Count;j++) actionChances[botActionManualCategories[i][j].botActionManualIndex] = presummedActionChances[i];
	}
	
	/// <summary>
	/// Pre-add the chance to a category.
	/// </summary>
	/// <param name="actionUse"></param>
	/// <param name="value"></param>
	public void PreSumActionChance(ActionUse actionUse, int value)
	{
		presummedActionChances[(int)actionUse] += value;
	}
	
	/// <summary>
	/// Add a set of manuals to the center manual list and
	/// handle their according chances and distribution for
	/// random selection purpose.
	/// </summary>
	/// <param name="p_botActionManuals"></param>
	public void AddManuals(List<BotActionManual> p_botActionManuals)
	{
		int i=botActionManuals.Count;
		for (int j=0;j<p_botActionManuals.Count;j++,i++)
		{
			p_botActionManuals[j].botActionManualIndex = i;
			botActionManuals.Add(p_botActionManuals[j]);
			botActionManualCategories[(int)p_botActionManuals[j].actionUse].Add(p_botActionManuals[j]);
			actionChances.Add(0);
			actionCumulativeDistribution.Add(0);
		}
	}

    public override void Start()
    {
        base.Start();
    }
}