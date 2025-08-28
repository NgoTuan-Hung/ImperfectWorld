using System.Collections;
using UnityEngine;

public partial class BaseAction
{
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

            GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions)
                .value[i]
                .action(Vector2.zero, customMono.GetDirection());
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
        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        customMono.rotationAndCenterObject.transform.localScale = new(
            customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
            1,
            1
        );
        customMono.rotationAndCenterObject.transform.Rotate(
            Vector3.forward,
            Vector2.SignedAngle(
                customMono.rotationAndCenterObject.transform.localScale,
                customMono.GetDirection()
            )
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
}
