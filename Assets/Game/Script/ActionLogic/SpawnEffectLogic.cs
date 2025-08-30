using System;
using UnityEngine;

public class SpawnEffectLogic : ActionLogic
{
    /// <summary>
    /// Use Damage
    /// </summary>
    /// <param name="baseAction"></param>
    public SpawnEffectLogic(BaseAction baseAction)
        : base(baseAction) { }

    public void SpawnEffectAsChild(
        Vector2 p_dir,
        GameEffect p_gameEffect,
        Action<float> p_dealDamageEvent = null
    )
    {
        baseAction.customMono.rotationAndCenterObject.transform.rotation = Quaternion.Euler(
            new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, p_dir))
        );
        baseAction.customMono.rotationAndCenterObject.transform.localScale =
            baseAction.customMono.rotationAndCenterObject.transform.localScale.WithY(
                baseAction.customMono.rotationAndCenterObject.transform.right.x > 0 ? 1 : -1
            );

        p_gameEffect.SetParentAndLocalPosAndRot(
            baseAction.customMono.rotationAndCenterObject.transform,
            p_gameEffect.gameEffectSO.effectLocalPosition,
            p_gameEffect.gameEffectSO.effectLocalRotation
        );
        p_gameEffect.SetUpCollideAndDamage(
            baseAction.customMono.allyTags,
            baseAction.GetActionField<ActionFloatField>(ActionFieldName.Damage).value,
            p_dealDamageEvent
        );
    }

    public void SpawnEffectAsChild(GameEffect p_gameEffect, Action<float> p_dealDamageEvent = null)
    {
        SpawnEffectAsChild(baseAction.customMono.GetDirection(), p_gameEffect, p_dealDamageEvent);
    }

    public void SpawnEffectRelative(GameEffect p_gameEffect, Action<float> p_dealDamageEvent = null)
    {
        SpawnEffectAsChild(p_gameEffect, p_dealDamageEvent);
        p_gameEffect.transform.parent = null;
    }
}
