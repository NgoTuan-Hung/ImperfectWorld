using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// method in here can be reused for different actions/skill
/// </summary>
public partial class ActionLogicDataBase : MonoBehaviour
{
    public static void NoAction() { }

    public void ConfigCombo1(BaseAction p_baseAction)
    {
        for (
            int i = 0;
            i < p_baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            p_baseAction
                .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value.Add(new());
        }
    }

    /// <summary>
    /// Use Variants, ComboEffects, ComboActions, ComboEndAction
    /// </summary>
    /// <param name="p_baseAction"></param>
    /// <param name="p_dir"></param>
    /// <returns></returns>
    public IEnumerator WaitCombo1(BaseAction p_baseAction, Vector3 p_dir)
    {
        for (
            int i = 0;
            i < p_baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            while (!p_baseAction.customMono.animationEventFunctionCaller.combo1Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            p_baseAction.customMono.animationEventFunctionCaller.combo1Signal = false;

            if (
                p_baseAction
                    .GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectPool != null
            )
            {
                p_baseAction
                    .GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectAction(
                        p_baseAction,
                        p_baseAction
                            .GetActionField<ActionListComboEffectField>(
                                ActionFieldName.ComboEffects
                            )
                            .value[i]
                            .effectPool.PickOneGameEffect(),
                        null
                    );
            }

            StartCoroutine(
                p_baseAction
                    .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionIE = p_baseAction
                    .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionMethod(Vector2.zero, p_baseAction.customMono.GetDirection())
            );
        }

        while (!p_baseAction.customMono.animationEventFunctionCaller.endCombo1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        p_baseAction.GetActionField<ActionActionField>(ActionFieldName.ComboEndAction).value();
    }

    /// <summary>
    /// Should be called when Action is stoppped unintentionally
    /// </summary>
    public void Combo1Stop(BaseAction p_baseAction)
    {
        p_baseAction
            .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
            .value.ForEach(cA =>
            {
                if (cA.actionIE != null)
                    StopCoroutine(cA.actionIE);
            });
    }

    public void ConfigCombo2(BaseAction p_baseAction)
    {
        for (
            int i = 0;
            i < p_baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            p_baseAction
                .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value.Add(new());
        }
    }

    /// <summary>
    /// Use Variants, ComboEffects, ComboActions, ComboEndAction
    /// </summary>
    /// <param name="p_baseAction"></param>
    /// <param name="p_dir"></param>
    /// <returns></returns>
    public IEnumerator WaitCombo2(BaseAction p_baseAction, Vector3 p_dir)
    {
        for (
            int i = 0;
            i < p_baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            while (!p_baseAction.customMono.animationEventFunctionCaller.combo2Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            p_baseAction.customMono.animationEventFunctionCaller.combo2Signal = false;

            if (
                p_baseAction
                    .GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectPool != null
            )
            {
                p_baseAction
                    .GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectAction(
                        p_baseAction,
                        p_baseAction
                            .GetActionField<ActionListComboEffectField>(
                                ActionFieldName.ComboEffects
                            )
                            .value[i]
                            .effectPool.PickOneGameEffect(),
                        null
                    );
            }

            StartCoroutine(
                p_baseAction
                    .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionIE = p_baseAction
                    .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionMethod(Vector2.zero, p_baseAction.customMono.GetDirection())
            );
        }

        while (!p_baseAction.customMono.animationEventFunctionCaller.endCombo2)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        p_baseAction.GetActionField<ActionActionField>(ActionFieldName.ComboEndAction).value();
    }

    /// <summary>
    /// Should be called when Action is stoppped unintentionally
    /// </summary>
    public void Combo2Stop(BaseAction p_baseAction)
    {
        p_baseAction
            .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
            .value.ForEach(cA =>
            {
                if (cA.actionIE != null)
                    StopCoroutine(cA.actionIE);
            });
    }

    /// <summary>
    /// This use CurrentTime.
    /// </summary>
    /// <param name="p_dir"></param>
    /// <param name="p_spped"></param>
    /// <param name="p_duration"></param>
    /// <param name="p_easing"></param>
    /// <returns></returns>
    public IEnumerator Dash(
        BaseAction p_baseAction,
        Vector2 p_dir,
        float p_spped,
        float p_duration,
        Func<float, float> p_easing
    )
    {
        p_baseAction.GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value = 0;
        p_dir = p_dir.normalized * p_spped;
        while (
            p_baseAction.GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
            < p_duration
        )
        {
            p_baseAction.transform.AddPos(
                p_dir
                    * p_easing(
                        1
                            - p_baseAction
                                .GetActionField<ActionFloatField>(ActionFieldName.CurrentTime)
                                .value / p_duration
                    )
            );

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            p_baseAction.GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value +=
                Time.fixedDeltaTime;
        }
    }

    public IEnumerator Flash(BaseAction p_baseAction, Vector2 p_dir, float p_dist, float p_duration)
    {
        p_baseAction.customMono.boxCollider2D.enabled = false;
        p_baseAction.customMono.combatCollider2D.enabled = false;
        p_baseAction.customMono.spriteRenderer.enabled = false;
        p_baseAction.transform.AddPos(p_dir.normalized * p_dist);

        yield return new WaitForSeconds(p_duration);

        p_baseAction.customMono.boxCollider2D.enabled = true;
        p_baseAction.customMono.combatCollider2D.enabled = true;
        p_baseAction.customMono.spriteRenderer.enabled = true;
    }
}
