using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    bool actionNeedWait = false;
    BotActionManual waitBotActionManual;

    public virtual void Awake()
    {
        customMono = GetComponent<CustomMono>();
        var t_actionUses = Enum.GetNames(typeof(ActionUse));
        for (int i = 0; i < t_actionUses.Length; i++)
        {
            botActionManualCategories.Add(new List<BotActionManual>());
            presummedActionChances.Add(0);
        }
    }

    public virtual void InitAction() { }

    /// <summary>
    /// Normalize action chances and calculate cumulative distribution of
    /// each action.
    /// </summary>
    void RecalculateActionCumulativeDistribution()
    {
        float t_total = actionChances.Sum();

        actionCumulativeDistribution[0] = actionChances[0] / t_total;
        for (int i = 1; i < actionCumulativeDistribution.Count; i++)
        {
            actionCumulativeDistribution[i] =
                actionCumulativeDistribution[i - 1] + actionChances[i] / t_total;
        }
    }

    /// <summary>
    /// So you have cumulativeDistribution of 0.3 0.9 1, that means 0-0.3 is the distribution of
    /// action 0 and it has 30% chance, 0.3-0.9 is the distribution of action 1 and it has 60% chance
    /// and so on. To pick a random action, we can search if our random value is less than any
    /// cumulative distribution while we iterate from left to right, or we can do a binary search.
    /// </summary>
    /// <returns></returns>
    public void ExecuteAnyActionThisFrame(
        bool actionInterval,
        Vector2 p_originToTargetOriginDirection = default,
        Vector2 p_centerToTargetCenterDirection = default,
        Vector2 p_firePointToTargetCenterDirection = default,
        Vector2 p_targetOriginPosition = default,
        Vector2 p_targetCenterPosition = default
    )
    {
        if (actionNeedWait)
        {
            HandleActionRequiredParameter(
                waitBotActionManual,
                p_originToTargetOriginDirection,
                p_centerToTargetCenterDirection,
                p_firePointToTargetCenterDirection,
                p_targetOriginPosition,
                p_targetCenterPosition
            );
            waitBotActionManual.whileWaiting(
                waitBotActionManual.doActionParamInfo.targetOriginPosition,
                waitBotActionManual.doActionParamInfo.originToTargetOriginDirection
            );
            return;
        }

        if (actionInterval)
        {
            RefreshActionChances();
            return;
        }

        CalculatingFinalActionChances();
        RecalculateActionCumulativeDistribution();
        var rand = Random.Range(0, 1f);
        BotActionManual t_botActionManual = botActionManuals[
            actionCumulativeDistribution.CumulativeDistributionBinarySearch(
                0,
                actionCumulativeDistribution.Count - 1,
                rand
            )
        ];

        if (t_botActionManual.actionNeedWait)
        {
            HandleActionRequiredParameter(
                t_botActionManual,
                p_originToTargetOriginDirection,
                p_centerToTargetCenterDirection,
                p_firePointToTargetCenterDirection,
                p_targetOriginPosition,
                p_targetCenterPosition
            );
            if (t_botActionManual.startAndWait().success)
            {
                actionNeedWait = true;
                waitBotActionManual = t_botActionManual;
                StartCoroutine(
                    EndActionWaiting(
                        t_botActionManual.doActionParamInfo.nextActionChoosingIntervalProposal
                    )
                );
            }
        }
        else
        {
            HandleActionRequiredParameter(
                t_botActionManual,
                p_originToTargetOriginDirection,
                p_centerToTargetCenterDirection,
                p_firePointToTargetCenterDirection,
                p_targetOriginPosition,
                p_targetCenterPosition
            );
            t_botActionManual.doAction(t_botActionManual.doActionParamInfo);
        }

        RefreshActionChances();
    }

    IEnumerator EndActionWaiting(float duration)
    {
        yield return new WaitForSeconds(duration);
        actionNeedWait = false;
        RefreshActionChances();
        waitBotActionManual.doAction(waitBotActionManual.doActionParamInfo);
    }

    /// <summary>
    /// Set all the value to 0
    /// </summary>
    public void RefreshActionChances()
    {
        for (int i = 0; i < actionChances.Count; i++)
            actionChances[i] = 0;
        for (int i = 0; i < presummedActionChances.Count; i++)
            presummedActionChances[i] = 0;
    }

    /// <summary>
    /// Loop through each category only once and set the final action chance
    /// of each action.
    /// </summary>
    void CalculatingFinalActionChances()
    {
        for (int i = 0; i < botActionManualCategories.Count; i++)
        for (int j = 0; j < botActionManualCategories[i].Count; j++)
            actionChances[botActionManualCategories[i][j].botActionManualIndex] =
                presummedActionChances[i] + botActionManualCategories[i][j].actionChanceAjuster;
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
        int i = botActionManuals.Count;
        for (int j = 0; j < p_botActionManuals.Count; j++, i++)
        {
            p_botActionManuals[j].botActionManualIndex = i;
            botActionManuals.Add(p_botActionManuals[j]);
            botActionManualCategories[(int)p_botActionManuals[j].actionUse]
                .Add(p_botActionManuals[j]);
            actionChances.Add(0);
            actionCumulativeDistribution.Add(0);
        }
    }

    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Prepare all the required parameters for the action
    /// to use it properly, for example, if we need to shoot
    /// someone we need to know where the target is, should
    /// we headshot or not.
    /// </summary>
    void HandleActionRequiredParameter(
        BotActionManual botActionManual,
        Vector2 p_originToTargetOriginDirection = default,
        Vector2 p_centerToTargetCenterDirection = default,
        Vector2 p_firePointToTargetCenterDirection = default,
        Vector2 p_targetOriginPosition = default,
        Vector2 p_targetCenterPosition = default
    )
    {
        botActionManual.doActionParamInfo.originToTargetOriginDirection =
            p_originToTargetOriginDirection;
        botActionManual.doActionParamInfo.centerToTargetCenterDirection =
            p_centerToTargetCenterDirection;
        botActionManual.doActionParamInfo.firePointToTargetCenterDirection =
            p_firePointToTargetCenterDirection;
        botActionManual.doActionParamInfo.targetOriginPosition = p_targetOriginPosition;
        botActionManual.doActionParamInfo.targetCenterPosition = p_targetCenterPosition;
        if (botActionManual.doActionParamInfo.isDirectionModify)
        {
            botActionManual.doActionParamInfo.originToTargetOriginDirection *= botActionManual
                .doActionParamInfo
                .directionModifier;
            botActionManual.doActionParamInfo.centerToTargetCenterDirection *= botActionManual
                .doActionParamInfo
                .directionModifier;
            botActionManual.doActionParamInfo.firePointToTargetCenterDirection *= botActionManual
                .doActionParamInfo
                .directionModifier;
        }
    }
}
