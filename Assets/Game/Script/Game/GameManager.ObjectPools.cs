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
        rimuruCombo2SlashBPool;

    void InitAllEffectPools()
    {
        LoadGameEffectPrefabs();
        moonSlashExplodePool = new(
            moonSlashExplode,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        magicLaserImpactPool = new(
            magicLaserImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        slaughterProjectilePool = new(
            slaughterProjectile,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        slaughterExplosionPool = new(
            slaughterExplosion,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        strongDudePunchImpactPool = new(
            strongDudePunchImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        samuraiSlashPool = new(
            samuraiSlash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfPhongTornadoImpactPool = new(
            bladeOfPhongTornadoImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfVuImpactPool = new(
            bladeOfVuImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        arrowPool = new(
            arrow,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerArrowPool = new(
            elementalLeafRangerArrow,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerPoisonArrowImpactPool = new(
            elementalLeafRangerPoisonArrowImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerVineArrowImpactPool = new(
            elementalLeafRangerVineArrowImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        wanderMagicianProjectilePool = new(
            wanderMagicianProjectile,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        vanishEffectPool = new(
            vanishEffect,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfMinhKhaiSlashEffectPool = new(
            bladeOfMinhKhaiSlashEffect,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfPhongTornadoEffectPool = new(
            bladeOfPhongTornadoEffect,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        ghostPool = new(
            ghost,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfVuStarPool = new(
            bladeOfVuStar,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfVuSlashPool = new(
            bladeOfVuSlash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        pierceStrikePool = new(
            pierceStrike,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        pierceStrikeSecondPhasePool = new(
            pierceStrikeSecondPhase,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        deepBladeSlashPool = new(
            deepBladeSlash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rayOfJungleBeamPool = new(
            rayOfJungleBeam,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        woodCryArrowPool = new(
            woodCryArrow,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerPoisonArrowPool = new(
            elementalLeafRangerPoisonArrow,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerVineArrowPool = new(
            elementalLeafRangerVineArrow,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        lightingForwardLightingPool = new(
            lightingForwardLighting,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        blueHolePool = new(
            blueHole,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        nuclearBombExplosionPool = new(
            nuclearBombExplosion,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        swordTempestSlash1Pool = new(
            swordTempestSlash1,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        swordTempestSlash2Pool = new(
            swordTempestSlash2,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        swordTempestSlash3Pool = new(
            swordTempestSlash3,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        dashEffectPool = new(
            dashEffect,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        dashExplodePool = new(
            dashExplode,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        magicLaserPool = new(
            magicLaser,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        moonSlashPool = new(
            moonSlash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        scatterArrowPool = new(
            scatterArrow,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        scatterChargePool = new(
            scatterCharge,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        attackColliderPool = new(
            attackCollider,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        dieDissolvePool = new(
            dieDissolve,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        strongDudeShockwavePool = new(
            strongDudeShockwave,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        knockUpColliderPool = new(
            knockUpCollider,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        pushRandomColliderPool = new(
            pushRandomCollider,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        pushColliderPool = new(
            pushCollider,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        stunColliderPool = new(
            stunCollider,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        phantomPulseDragonPool = new(
            phantomPulseDragon,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        infernalTideFlamePool = new(
            infernalTideFlame,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        infernalTideFlameNoReceiverPool = new(
            infernalTideFlameNoReceiver,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        infernalTideFanPool = new(
            infernalTideFan,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        scatterFlashPool = new(
            scatterFlash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        stormFangMergeBladesPool = new(
            stormFangMergeBlades,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        stormFangMergeProgressPool = new(
            stormFangMergeProgress,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        kunaiPool = new(
            kunai,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        kunaiHitPool = new(
            kunaiHit,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        arcaneSwarmSlash1Pool = new(
            arcaneSwarmSlash1,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        arcaneSwarmSlash2Pool = new(
            arcaneSwarmSlash2,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        arcaneSwarmSlash3Pool = new(
            arcaneSwarmSlash3,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        arcaneSwarmSlash4Pool = new(
            arcaneSwarmSlash4,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        arcaneSwarmSlash5Pool = new(
            arcaneSwarmSlash5,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        orbitalNemesisDashPool = new(
            orbitalNemesisDash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        heliosGazeRayPool = new(
            heliosGazeRay,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rimuruCombo2SlashAPool = new(
            rimuruCombo2SlashA,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rimuruCombo2SlashBPool = new(
            rimuruCombo2SlashB,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rimuruCombo1SlashAPool = new(
            rimuruCombo1SlashA,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rimuruCombo1SlashBPool = new(
            rimuruCombo1SlashB,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rimuruCombo1DashPool = new(
            rimuruCombo1Dash,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rimuruCombo1DashImpactPool = new(
            rimuruCombo1DashImpact,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        LinkPoolWithScriptableObject();
    }

    void LinkPoolWithScriptableObject()
    {
        poolLink.Add(
            moonSlashExplode.GetComponent<GameEffect>().gameEffectSO,
            moonSlashExplodePool
        );
        poolLink.Add(magicLaserImpact.GetComponent<GameEffect>().gameEffectSO, magicLaserPool);
        poolLink.Add(
            slaughterProjectile.GetComponent<GameEffect>().gameEffectSO,
            slaughterProjectilePool
        );
        poolLink.Add(
            slaughterExplosion.GetComponent<GameEffect>().gameEffectSO,
            slaughterExplosionPool
        );
        poolLink.Add(
            strongDudePunchImpact.GetComponent<GameEffect>().gameEffectSO,
            strongDudePunchImpactPool
        );
        poolLink.Add(samuraiSlash.GetComponent<GameEffect>().gameEffectSO, samuraiSlashPool);
        poolLink.Add(
            bladeOfPhongTornadoImpact.GetComponent<GameEffect>().gameEffectSO,
            bladeOfPhongTornadoImpactPool
        );
        poolLink.Add(bladeOfVuImpact.GetComponent<GameEffect>().gameEffectSO, bladeOfVuImpactPool);
        poolLink.Add(arrow.GetComponent<GameEffect>().gameEffectSO, arrowPool);
        poolLink.Add(
            elementalLeafRangerArrow.GetComponent<GameEffect>().gameEffectSO,
            elementalLeafRangerArrowPool
        );
        poolLink.Add(
            elementalLeafRangerPoisonArrowImpact.GetComponent<GameEffect>().gameEffectSO,
            elementalLeafRangerPoisonArrowImpactPool
        );
        poolLink.Add(
            elementalLeafRangerVineArrowImpact.GetComponent<GameEffect>().gameEffectSO,
            elementalLeafRangerVineArrowImpactPool
        );
        poolLink.Add(
            wanderMagicianProjectile.GetComponent<GameEffect>().gameEffectSO,
            wanderMagicianProjectilePool
        );
        poolLink.Add(vanishEffect.GetComponent<GameEffect>().gameEffectSO, vanishEffectPool);
        poolLink.Add(
            bladeOfMinhKhaiSlashEffect.GetComponent<GameEffect>().gameEffectSO,
            bladeOfMinhKhaiSlashEffectPool
        );
        poolLink.Add(
            bladeOfPhongTornadoEffect.GetComponent<GameEffect>().gameEffectSO,
            bladeOfPhongTornadoEffectPool
        );
        poolLink.Add(ghost.GetComponent<GameEffect>().gameEffectSO, ghostPool);
        poolLink.Add(bladeOfVuStar.GetComponent<GameEffect>().gameEffectSO, bladeOfVuStarPool);
        poolLink.Add(bladeOfVuSlash.GetComponent<GameEffect>().gameEffectSO, bladeOfVuSlashPool);
        poolLink.Add(pierceStrike.GetComponent<GameEffect>().gameEffectSO, pierceStrikePool);
        poolLink.Add(
            pierceStrikeSecondPhase.GetComponent<GameEffect>().gameEffectSO,
            pierceStrikeSecondPhasePool
        );
        poolLink.Add(deepBladeSlash.GetComponent<GameEffect>().gameEffectSO, deepBladeSlashPool);
        poolLink.Add(rayOfJungleBeam.GetComponent<GameEffect>().gameEffectSO, rayOfJungleBeamPool);
        poolLink.Add(woodCryArrow.GetComponent<GameEffect>().gameEffectSO, woodCryArrowPool);
        poolLink.Add(
            elementalLeafRangerPoisonArrow.GetComponent<GameEffect>().gameEffectSO,
            elementalLeafRangerPoisonArrowPool
        );
        poolLink.Add(
            elementalLeafRangerVineArrow.GetComponent<GameEffect>().gameEffectSO,
            elementalLeafRangerVineArrowPool
        );
        poolLink.Add(
            lightingForwardLighting.GetComponent<GameEffect>().gameEffectSO,
            lightingForwardLightingPool
        );
        poolLink.Add(blueHole.GetComponent<GameEffect>().gameEffectSO, blueHolePool);
        poolLink.Add(
            nuclearBombExplosion.GetComponent<GameEffect>().gameEffectSO,
            nuclearBombExplosionPool
        );
        poolLink.Add(
            swordTempestSlash1.GetComponent<GameEffect>().gameEffectSO,
            swordTempestSlash1Pool
        );
        poolLink.Add(
            swordTempestSlash2.GetComponent<GameEffect>().gameEffectSO,
            swordTempestSlash2Pool
        );
        poolLink.Add(
            swordTempestSlash3.GetComponent<GameEffect>().gameEffectSO,
            swordTempestSlash3Pool
        );
        poolLink.Add(dashEffect.GetComponent<GameEffect>().gameEffectSO, dashEffectPool);
        poolLink.Add(dashExplode.GetComponent<GameEffect>().gameEffectSO, dashExplodePool);
        poolLink.Add(magicLaser.GetComponent<GameEffect>().gameEffectSO, magicLaserPool);
        poolLink.Add(moonSlash.GetComponent<GameEffect>().gameEffectSO, moonSlashPool);
        poolLink.Add(scatterArrow.GetComponent<GameEffect>().gameEffectSO, scatterArrowPool);
        poolLink.Add(scatterCharge.GetComponent<GameEffect>().gameEffectSO, scatterChargePool);
        poolLink.Add(attackCollider.GetComponent<GameEffect>().gameEffectSO, attackColliderPool);
        poolLink.Add(dieDissolve.GetComponent<GameEffect>().gameEffectSO, dieDissolvePool);
        poolLink.Add(
            strongDudeShockwave.GetComponent<GameEffect>().gameEffectSO,
            strongDudeShockwavePool
        );
        poolLink.Add(knockUpCollider.GetComponent<GameEffect>().gameEffectSO, knockUpColliderPool);
        poolLink.Add(
            pushRandomCollider.GetComponent<GameEffect>().gameEffectSO,
            pushRandomColliderPool
        );
        poolLink.Add(pushCollider.GetComponent<GameEffect>().gameEffectSO, pushColliderPool);
        poolLink.Add(stunCollider.GetComponent<GameEffect>().gameEffectSO, stunColliderPool);
        poolLink.Add(
            phantomPulseDragon.GetComponent<GameEffect>().gameEffectSO,
            phantomPulseDragonPool
        );
        poolLink.Add(
            infernalTideFlame.GetComponent<GameEffect>().gameEffectSO,
            infernalTideFlamePool
        );
        poolLink.Add(
            infernalTideFlameNoReceiver.GetComponent<GameEffect>().gameEffectSO,
            infernalTideFlameNoReceiverPool
        );
        poolLink.Add(infernalTideFan.GetComponent<GameEffect>().gameEffectSO, infernalTideFanPool);
        poolLink.Add(scatterFlash.GetComponent<GameEffect>().gameEffectSO, scatterFlashPool);
        poolLink.Add(
            stormFangMergeBlades.GetComponent<GameEffect>().gameEffectSO,
            stormFangMergeBladesPool
        );
        poolLink.Add(
            stormFangMergeProgress.GetComponent<GameEffect>().gameEffectSO,
            stormFangMergeProgressPool
        );
        poolLink.Add(kunai.GetComponent<GameEffect>().gameEffectSO, kunaiPool);
        poolLink.Add(kunaiHit.GetComponent<GameEffect>().gameEffectSO, kunaiHitPool);
        poolLink.Add(
            arcaneSwarmSlash1.GetComponent<GameEffect>().gameEffectSO,
            arcaneSwarmSlash1Pool
        );
        poolLink.Add(
            arcaneSwarmSlash2.GetComponent<GameEffect>().gameEffectSO,
            arcaneSwarmSlash2Pool
        );
        poolLink.Add(
            arcaneSwarmSlash3.GetComponent<GameEffect>().gameEffectSO,
            arcaneSwarmSlash3Pool
        );
        poolLink.Add(
            arcaneSwarmSlash4.GetComponent<GameEffect>().gameEffectSO,
            arcaneSwarmSlash4Pool
        );
        poolLink.Add(
            arcaneSwarmSlash5.GetComponent<GameEffect>().gameEffectSO,
            arcaneSwarmSlash5Pool
        );
        poolLink.Add(
            orbitalNemesisDash.GetComponent<GameEffect>().gameEffectSO,
            orbitalNemesisDashPool
        );
        poolLink.Add(heliosGazeRay.GetComponent<GameEffect>().gameEffectSO, heliosGazeRayPool);
        poolLink.Add(
            rimuruCombo2SlashA.GetComponent<GameEffect>().gameEffectSO,
            rimuruCombo2SlashAPool
        );
        poolLink.Add(
            rimuruCombo2SlashB.GetComponent<GameEffect>().gameEffectSO,
            rimuruCombo2SlashBPool
        );
        poolLink.Add(
            rimuruCombo1SlashA.GetComponent<GameEffect>().gameEffectSO,
            rimuruCombo1SlashAPool
        );
        poolLink.Add(
            rimuruCombo1SlashB.GetComponent<GameEffect>().gameEffectSO,
            rimuruCombo1SlashBPool
        );
        poolLink.Add(
            rimuruCombo1Dash.GetComponent<GameEffect>().gameEffectSO,
            rimuruCombo1DashPool
        );
        poolLink.Add(
            rimuruCombo1DashImpact.GetComponent<GameEffect>().gameEffectSO,
            rimuruCombo1DashImpactPool
        );
    }
}
