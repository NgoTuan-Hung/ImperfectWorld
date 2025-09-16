using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class GameManager
{
    public ObjectPool moonSlashExplodePool,
        magicLaserImpactPool,
        slaughterProjectilePool,
        slaughterExplosionPool,
        strongDudePunchImpactPool,
        samuraiSlashPool,
        bladeOfPhongTornadoImpactPool,
        bladeOfVuImpactPool,
        arrowPool,
        elementalLeafRangerArrowPool,
        elementalLeafRangerPoisonArrowImpactPool,
        elementalLeafRangerVineArrowImpactPool,
        wanderMagicianProjectilePool,
        vanishEffectPool,
        bladeOfMinhKhaiSlashEffectPool,
        bladeOfPhongTornadoEffectPool,
        ghostPool,
        bladeOfVuStarPool,
        bladeOfVuSlashPool,
        pierceStrikePool,
        pierceStrikeSecondPhasePool,
        deepBladeSlashPool,
        rayOfJungleBeamPool,
        woodCryArrowPool,
        elementalLeafRangerPoisonArrowPool,
        elementalLeafRangerVineArrowPool,
        lightingForwardLightingPool,
        blueHolePool,
        nuclearBombExplosionPool,
        swordTempestSlash1Pool,
        swordTempestSlash2Pool,
        swordTempestSlash3Pool,
        dashEffectPool,
        dashExplodePool,
        magicLaserPool,
        moonSlashPool,
        scatterArrowPool,
        scatterChargePool,
        attackColliderPool,
        dieDissolvePool,
        strongDudeShockwavePool,
        knockUpColliderPool,
        pushRandomColliderPool,
        pushColliderPool,
        stunColliderPool,
        phantomPulseDragonPool,
        infernalTideFlamePool,
        infernalTideFlameNoReceiverPool,
        infernalTideFanPool,
        scatterFlashPool,
        stormFangMergeBladesPool,
        stormFangMergeProgressPool,
        kunaiPool,
        kunaiHitPool,
        arcaneSwarmSlash1Pool,
        arcaneSwarmSlash2Pool,
        arcaneSwarmSlash3Pool,
        arcaneSwarmSlash4Pool,
        arcaneSwarmSlash5Pool,
        orbitalNemesisDashPool,
        heliosGazeRayPool,
        rimuruCombo1SlashAPool,
        rimuruCombo1SlashBPool,
        rimuruCombo1DashPool,
        rimuruCombo1DashImpactPool,
        rimuruCombo2SlashAPool,
        rimuruCombo2SlashBPool,
        sovereignFlowEffectPool,
        rimuruFireBallPool,
        rimuruFireBallImpactPool,
        rimuruLightingWolfPool,
        rimuruLightingWolfSummonCirclePool,
        rimuruHeavenFallRayPool,
        virgilAttackImpactPool,
        virgilAttackSlashPool;
    List<string> objectPoolNames = new()
    {
        "moonSlashExplodePool",
        "magicLaserImpactPool",
        "slaughterProjectilePool",
        "slaughterExplosionPool",
        "strongDudePunchImpactPool",
        "samuraiSlashPool",
        "bladeOfPhongTornadoImpactPool",
        "bladeOfVuImpactPool",
        "arrowPool",
        "elementalLeafRangerArrowPool",
        "elementalLeafRangerPoisonArrowImpactPool",
        "elementalLeafRangerVineArrowImpactPool",
        "wanderMagicianProjectilePool",
        "vanishEffectPool",
        "bladeOfMinhKhaiSlashEffectPool",
        "bladeOfPhongTornadoEffectPool",
        "ghostPool",
        "bladeOfVuStarPool",
        "bladeOfVuSlashPool",
        "pierceStrikePool",
        "pierceStrikeSecondPhasePool",
        "deepBladeSlashPool",
        "rayOfJungleBeamPool",
        "woodCryArrowPool",
        "elementalLeafRangerPoisonArrowPool",
        "elementalLeafRangerVineArrowPool",
        "lightingForwardLightingPool",
        "blueHolePool",
        "nuclearBombExplosionPool",
        "swordTempestSlash1Pool",
        "swordTempestSlash2Pool",
        "swordTempestSlash3Pool",
        "dashEffectPool",
        "dashExplodePool",
        "magicLaserPool",
        "moonSlashPool",
        "scatterArrowPool",
        "scatterChargePool",
        "attackColliderPool",
        "dieDissolvePool",
        "strongDudeShockwavePool",
        "knockUpColliderPool",
        "pushRandomColliderPool",
        "pushColliderPool",
        "stunColliderPool",
        "phantomPulseDragonPool",
        "infernalTideFlamePool",
        "infernalTideFlameNoReceiverPool",
        "infernalTideFanPool",
        "scatterFlashPool",
        "stormFangMergeBladesPool",
        "stormFangMergeProgressPool",
        "kunaiPool",
        "kunaiHitPool",
        "arcaneSwarmSlash1Pool",
        "arcaneSwarmSlash2Pool",
        "arcaneSwarmSlash3Pool",
        "arcaneSwarmSlash4Pool",
        "arcaneSwarmSlash5Pool",
        "orbitalNemesisDashPool",
        "heliosGazeRayPool",
        "rimuruCombo1SlashAPool",
        "rimuruCombo1SlashBPool",
        "rimuruCombo1DashPool",
        "rimuruCombo1DashImpactPool",
        "rimuruCombo2SlashAPool",
        "rimuruCombo2SlashBPool",
        "sovereignFlowEffectPool",
        "rimuruFireBallPool",
        "rimuruFireBallImpactPool",
        "rimuruLightingWolfPool",
        "rimuruLightingWolfSummonCirclePool",
        "rimuruHeavenFallRayPool",
        "virgilAttackImpactPool",
        "virgilAttackSlashPool",
    };

    void InitAllEffectPools()
    {
        LoadGameEffectPrefabs();
        Type type = GetType();
        foreach (var name in objectPoolNames)
        {
            FieldInfo field = type.GetField(
                name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            /* Create pools from prefabs */
            var t_prefab =
                type.GetField(
                        name[..^4],
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    )
                    .GetValue(this) as GameObject;
            field.SetValue(
                this,
                new ObjectPool(
                    t_prefab,
                    new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
                )
            );

            /* Link Pools with ScriptableObjects */
            poolLink.Add(
                t_prefab.GetComponent<GameEffect>().gameEffectSO,
                (ObjectPool)field.GetValue(this)
            );
        }
    }
}
