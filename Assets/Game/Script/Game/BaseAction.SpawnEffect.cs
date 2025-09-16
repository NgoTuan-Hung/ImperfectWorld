using System;
using UnityEngine;

public partial class BaseAction
{
    public void SpawnEffectAtLoc(Vector2 p_pos, GameEffect p_gameEffect)
    {
        p_gameEffect.transform.position = p_pos;
    }

    public void SpawnEffectAsChild(Vector2 p_dir, GameEffect p_gameEffect)
    {
        SetRotationAndCenterObject(p_dir);

        p_gameEffect.SetParentAndLocalPosAndRot(
            customMono.rotationAndCenterObject.transform,
            p_gameEffect.gameEffectSO.effectLocalPosition,
            p_gameEffect.gameEffectSO.effectLocalRotation
        );
    }

    public void SpawnBasicCombatEffectAsChild(Vector2 p_dir, GameEffect p_gameEffect)
    {
        SpawnEffectAsChild(p_dir, p_gameEffect);
        p_gameEffect.SetUpCollideAndDamage(
            customMono.allyTags,
            GetActionField<ActionFloatField>(ActionFieldName.Damage).value
        );
    }

    /// <summary>
    /// Use Damage
    /// </summary>
    /// <param name="p_baseAction"></param>
    /// <param name="p_dir"></param>
    /// <param name="p_gameEffect"></param>
    /// <param name="p_dealDamageEvent"></param>
    public void SpawnBasicCombatEffectAsChild(
        Vector2 p_dir,
        GameEffect p_gameEffect,
        Action<CollideAndDamage> p_setupCollideAndDamage
    )
    {
        SpawnBasicCombatEffectAsChild(p_dir, p_gameEffect);
        p_setupCollideAndDamage(
            p_gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage
        );
    }

    public void SpawnEffectAsChild(
        GameEffect p_gameEffect,
        Action<CollideAndDamage> p_setupCollideAndDamage
    )
    {
        SpawnBasicCombatEffectAsChild(
            customMono.GetDirection(),
            p_gameEffect,
            p_setupCollideAndDamage
        );
    }

    public void SpawnEffectRelative(
        GameEffect p_gameEffect,
        Action<CollideAndDamage> p_setupCollideAndDamage
    )
    {
        SpawnEffectAsChild(p_gameEffect, p_setupCollideAndDamage);
        p_gameEffect.transform.parent = null;
    }

    public void SpawnNormalEffect(
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
                customMono.allyTags,
                GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );
    }
}
