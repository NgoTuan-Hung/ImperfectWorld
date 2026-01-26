using System;
using System.Collections;
using UnityEngine;

public partial class BaseAction
{
    public static void NoAction() { }

    public void ConfigCombo1()
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value.Add(new());
        }
    }

    /// <summary>
    /// Use Variants, ComboEffects, ComboActions, ComboEndAction
    /// </summary>
    /// <param name=""></param>
    /// <param name="p_dir"></param>
    /// <returns></returns>
    public IEnumerator WaitCombo1(Vector3 p_dir)
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            while (
                !customMono.animationEventFunctionCaller.GetSignalVals(
                    EAnimationSignal.Combo1Signal
                )
            )
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.Combo1Signal, false);

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
                            .effectPool.PickOneGameEffect(),
                        DefaultSetupCAD
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

        while (!customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndCombo1))
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        GetActionField<ActionActionField>(ActionFieldName.ComboEndAction).value();
    }

    /// <summary>
    /// Should be called when Action is stoppped unintentionally
    /// </summary>
    public void Combo1Stop()
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

    /// <summary>
    /// Use Variants, ComboEffects, ComboActions, ComboEndAction
    /// </summary>
    /// <param name=""></param>
    /// <param name="p_dir"></param>
    /// <returns></returns>
    public IEnumerator WaitCombo2(Vector3 p_dir)
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            while (
                !customMono.animationEventFunctionCaller.GetSignalVals(
                    EAnimationSignal.Combo2Signal
                )
            )
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.Combo2Signal, false);

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
                            .effectPool.PickOneGameEffect(),
                        DefaultSetupCAD
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

        while (!customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndCombo2))
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        GetActionField<ActionActionField>(ActionFieldName.ComboEndAction).value();
    }

    /// <summary>
    /// Should be called when Action is stoppped unintentionally
    /// </summary>
    public void Combo2Stop()
    {
        GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
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
        customMono.circleCollider2D.enabled = false;
        customMono.combatCollider2D.enabled = false;
        customMono.spriteRenderer.enabled = false;
        transform.AddPos(p_dir.normalized * p_dist);

        yield return new WaitForSeconds(p_duration);

        customMono.circleCollider2D.enabled = true;
        customMono.combatCollider2D.enabled = true;
        customMono.spriteRenderer.enabled = true;
    }

    public void DefaultSetupCAD(CollideAndDamage p_cAD) { }

    public void SetRotationAndCenterObject(Vector2 p_dir)
    {
        customMono.rotationAndCenterObject.transform.rotation = Quaternion.Euler(
            new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, p_dir))
        );
        customMono.rotationAndCenterObject.transform.localScale =
            customMono.rotationAndCenterObject.transform.localScale.WithY(
                customMono.rotationAndCenterObject.transform.right.x > 0 ? 1 : -1
            );
    }

    public void SetCombatProjectile(GameEffect p_gameEffect, Vector2 p_dir)
    {
        p_gameEffect.transform.SetPositionAndRotation(
            customMono.rotationAndCenterObject.transform.position,
            Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, p_dir)))
        );
        p_gameEffect.transform.localScale = p_gameEffect.transform.localScale.WithY(
            p_gameEffect.transform.right.x > 0 ? 1 : -1
        );

        p_gameEffect.SetUpCollideAndDamage(
            customMono,
            GetActionField<ActionFloatField>(ActionFieldName.Damage).value
        );

        p_gameEffect.KeepFlyingAt(p_dir);
    }
}
