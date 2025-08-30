using System;
using System.Collections;
using UnityEngine;

public class Combo1Logic : ActionLogic
{
    public Combo1Logic(BaseAction baseAction, Action p_combo1End)
        : base(baseAction)
    {
        combo1End = p_combo1End;
    }

    public Action combo1End = NoAction;

    public void ConfigCombo1()
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

    public IEnumerator WaitCombo1(Vector3 p_dir)
    {
        for (
            int i = 0;
            i < baseAction.GetActionField<ActionIntField>(ActionFieldName.Variants).value;
            i++
        )
        {
            while (!baseAction.customMono.animationEventFunctionCaller.combo1Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            baseAction.customMono.animationEventFunctionCaller.combo1Signal = false;

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

        while (!baseAction.customMono.animationEventFunctionCaller.endCombo1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        combo1End();
    }

    /// <summary>
    /// Should be called when Action is stoppped unintentionally
    /// </summary>
    public void Combo1Stop()
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
