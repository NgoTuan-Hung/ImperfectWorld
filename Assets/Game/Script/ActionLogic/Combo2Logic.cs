using System;
using System.Collections;
using UnityEngine;

public class Combo2Logic : ActionLogic
{
    public Combo2Logic(BaseAction baseAction, Action p_combo2End)
        : base(baseAction)
    {
        combo2End = p_combo2End;
    }

    public Action combo2End = NoAction;

    public void ConfigCombo2()
    {
        for (
            int i = 0;
            i < baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            baseAction
                .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value.Add(new());
        }
    }

    public IEnumerator WaitCombo2(Vector3 p_dir)
    {
        for (
            int i = 0;
            i < baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            while (!baseAction.customMono.animationEventFunctionCaller.combo2Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            baseAction.customMono.animationEventFunctionCaller.combo2Signal = false;

            if (
                baseAction
                    .GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectPool != null
            )
            {
                baseAction
                    .GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectAction(
                        baseAction
                            .GetActionField<ActionListComboEffectField>(
                                ActionFieldName.ComboEffects
                            )
                            .value[i]
                            .effectPool.PickOneGameEffect(),
                        null
                    );
            }

            baseAction.StartCoroutine(
                baseAction
                    .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionIE = baseAction
                    .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionMethod(Vector2.zero, baseAction.customMono.GetDirection())
            );
        }

        while (!baseAction.customMono.animationEventFunctionCaller.endCombo2)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        combo2End();
    }

    /// <summary>
    /// Should be called when Action is stoppped unintentionally
    /// </summary>
    public void Combo2Stop()
    {
        baseAction
            .GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
            .value.ForEach(cA =>
            {
                if (cA.actionIE != null)
                    baseAction.StopCoroutine(cA.actionIE);
            });
    }
}
