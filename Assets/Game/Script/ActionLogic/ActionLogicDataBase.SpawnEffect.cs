using System;
using UnityEngine;

public partial class ActionLogicDataBase
{
    /// <summary>
    /// Use Damage
    /// </summary>
    /// <param name="p_baseAction"></param>
    /// <param name="p_dir"></param>
    /// <param name="p_gameEffect"></param>
    /// <param name="p_dealDamageEvent"></param>
    public void SpawnEffectAsChild(
        BaseAction p_baseAction,
        Vector2 p_dir,
        GameEffect p_gameEffect,
        Action<float> p_dealDamageEvent = null
    )
    {
        p_baseAction.customMono.rotationAndCenterObject.transform.rotation = Quaternion.Euler(
            new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, p_dir))
        );
        p_baseAction.customMono.rotationAndCenterObject.transform.localScale =
            p_baseAction.customMono.rotationAndCenterObject.transform.localScale.WithY(
                p_baseAction.customMono.rotationAndCenterObject.transform.right.x > 0 ? 1 : -1
            );

        p_gameEffect.SetParentAndLocalPosAndRot(
            p_baseAction.customMono.rotationAndCenterObject.transform,
            p_gameEffect.gameEffectSO.effectLocalPosition,
            p_gameEffect.gameEffectSO.effectLocalRotation
        );
        p_gameEffect.SetUpCollideAndDamage(
            p_baseAction.customMono.allyTags,
            p_baseAction.GetActionField<ActionFloatField>(ActionFieldName.Damage).value,
            p_dealDamageEvent
        );
    }

    public void SpawnEffectAsChild(
        BaseAction p_baseAction,
        GameEffect p_gameEffect,
        Action<float> p_dealDamageEvent = null
    )
    {
        SpawnEffectAsChild(
            p_baseAction,
            p_baseAction.customMono.GetDirection(),
            p_gameEffect,
            p_dealDamageEvent
        );
    }

    public void SpawnEffectRelative(
        BaseAction p_baseAction,
        GameEffect p_gameEffect,
        Action<float> p_dealDamageEvent = null
    )
    {
        SpawnEffectAsChild(p_baseAction, p_gameEffect, p_dealDamageEvent);
        p_gameEffect.transform.parent = null;
    }

    public void SpawnNormalEffect(
        BaseAction p_baseAction,
        GameEffect p_gameEffect,
        Vector3 p_pos,
        Vector3 p_flyAtDir = default,
        bool p_isCombat = false
    )
    {
        p_gameEffect.transform.position = p_pos;
        if (p_flyAtDir != default)
            p_gameEffect.KeepFlyingAt(p_flyAtDir);

        if (p_isCombat)
            p_gameEffect.SetUpCollideAndDamage(
                p_baseAction.customMono.allyTags,
                p_baseAction.GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );
    }
}
