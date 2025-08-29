using System;
using System.Collections;
using UnityEngine;

public partial class BaseAction
{
    public void ConfigCombo1()
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value.Add(new());
        }
    }

    public IEnumerator WaitCombo1(Vector3 p_dir)
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            while (!customMono.animationEventFunctionCaller.combo1Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.animationEventFunctionCaller.combo1Signal = false;

            if (
                GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectPool != null
            )
            {
                GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectAction(
                        GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                            .value[i]
                            .effectPool.PickOneGameEffect()
                    );
            }

            StartCoroutine(
                GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionIE = GetActionField<ActionListComboActionField>(
                    ActionFieldName.ComboActions
                )
                    .value[i]
                    .actionMethod(Vector2.zero, customMono.GetDirection())
            );
        }

        while (!customMono.animationEventFunctionCaller.endCombo1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        Combo1End();
    }

    /// <summary>
    /// See WaitCombo1 for where this is called
    /// </summary>
    public virtual void Combo1End()
    {
        GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
            .value.ForEach(cA =>
            {
                if (cA.actionIE != null)
                    StopCoroutine(cA.actionIE);
            });
    }

    public void ConfigCombo2()
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value.Add(new());
        }
    }

    public IEnumerator WaitCombo2(Vector3 p_dir)
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            while (!customMono.animationEventFunctionCaller.combo2Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.animationEventFunctionCaller.combo2Signal = false;

            if (
                GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectPool != null
            )
            {
                GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                    .value[i]
                    .effectAction(
                        GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects)
                            .value[i]
                            .effectPool.PickOneGameEffect()
                    );
            }

            StartCoroutine(
                GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                    .value[i]
                    .actionIE = GetActionField<ActionListComboActionField>(
                    ActionFieldName.ComboActions
                )
                    .value[i]
                    .actionMethod(Vector2.zero, customMono.GetDirection())
            );
        }

        while (!customMono.animationEventFunctionCaller.endCombo2)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        Combo2End();
    }

    /// <summary>
    /// See WaitCombo2 for where this is called
    /// </summary>
    public virtual void Combo2End()
    {
        /*
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.endCombo2 = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.currentAction = null;
         */
    }

    public void SpawnEffectAsChild(GameEffect p_gameEffect)
    {
        customMono.rotationAndCenterObject.transform.rotation = Quaternion.Euler(
            new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, customMono.GetDirection()))
        );
        customMono.rotationAndCenterObject.transform.localScale =
            customMono.rotationAndCenterObject.transform.localScale.WithY(
                customMono.rotationAndCenterObject.transform.right.x > 0 ? 1 : -1
            );

        p_gameEffect.SetParentAndLocalPosAndRot(
            customMono.rotationAndCenterObject.transform,
            p_gameEffect.gameEffectSO.effectLocalPosition,
            p_gameEffect.gameEffectSO.effectLocalRotation
        );
        p_gameEffect.SetUpCollideAndDamage(
            customMono.allyTags,
            GetActionField<ActionFloatField>(ActionFieldName.Damage).value
        );
    }

    public void SpawnEffectRelative(GameEffect p_gameEffect)
    {
        SpawnEffectAsChild(p_gameEffect);
        p_gameEffect.transform.parent = null;
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
        Vector2 p_dir,
        float p_spped,
        float p_duration,
        Func<float, float> p_easing
    )
    {
        GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value = 0;
        p_dir = p_dir.normalized * p_spped;
        while (GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value < p_duration)
        {
            transform.AddPos(
                p_dir
                    * p_easing(
                        1
                            - GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
                                / p_duration
                    )
            );

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value +=
                Time.fixedDeltaTime;
        }
    }

    public IEnumerator Flash(Vector2 p_dir, float p_dist, float p_duration)
    {
        customMono.boxCollider2D.enabled = false;
        customMono.combatCollider2D.enabled = false;
        customMono.spriteRenderer.enabled = false;
        transform.AddPos(p_dir.normalized * p_dist);

        yield return new WaitForSeconds(p_duration);

        customMono.boxCollider2D.enabled = true;
        customMono.combatCollider2D.enabled = true;
        customMono.spriteRenderer.enabled = true;
    }
}
