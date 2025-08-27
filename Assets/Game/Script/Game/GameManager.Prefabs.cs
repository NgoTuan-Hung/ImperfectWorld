using UnityEngine;

public partial class GameManager
{
    GameObject moonSlashExplode,
        magicLaserImpact,
        slaughterProjectile,
        slaughterExplosion,
        strongDudePunchImpact,
        samuraiSlash,
        bladeOfPhongTornadoImpact,
        bladeOfVuImpact,
        arrow,
        elementalLeafRangerArrow,
        elementalLeafRangerPoisonArrowImpact,
        elementalLeafRangerVineArrowImpact,
        wanderMagicianProjectile,
        vanishEffect,
        bladeOfMinhKhaiSlashEffect,
        bladeOfPhongTornadoEffect,
        ghost,
        bladeOfVuStar,
        bladeOfVuSlash,
        pierceStrike,
        pierceStrikeSecondPhase,
        deepBladeSlash,
        rayOfJungleBeam,
        woodCryArrow,
        elementalLeafRangerPoisonArrow,
        elementalLeafRangerVineArrow,
        lightingForwardLighting,
        blueHole,
        nuclearBombExplosion,
        swordTempestSlash1,
        swordTempestSlash2,
        swordTempestSlash3,
        dashEffect,
        dashExplode,
        magicLaser,
        moonSlash,
        scatterArrow,
        scatterCharge,
        attackCollider,
        dieDissolve,
        strongDudeShockwave,
        knockUpCollider,
        pushRandomCollider,
        pushCollider,
        stunCollider,
        phantomPulseDragon,
        infernalTideFlame,
        infernalTideFlameNoReceiver,
        infernalTideFan,
        scatterFlash,
        stormFangMergeBlades,
        stormFangMergeProgress,
        kunai,
        kunaiHit,
        arcaneSwarmSlash1,
        arcaneSwarmSlash2,
        arcaneSwarmSlash3,
        arcaneSwarmSlash4,
        arcaneSwarmSlash5,
        orbitalNemesisDash,
        heliosGazeRay,
        rimuruCombo2SlashA,
        rimuruCombo2SlashB;

    void LoadGameEffectPrefabs()
    {
        moonSlashExplode = Resources.Load<GameObject>("MoonSlashExplode");
        magicLaserImpact = Resources.Load<GameObject>("MagicLaserImpact");
        slaughterProjectile = Resources.Load<GameObject>("SlaughterProjectile");
        slaughterExplosion = Resources.Load<GameObject>("SlaughterExplosion");
        strongDudePunchImpact = Resources.Load<GameObject>("StrongDudePunchImpact");
        samuraiSlash = Resources.Load<GameObject>("SamuraiSlash");
        bladeOfPhongTornadoImpact = Resources.Load<GameObject>("BladeOfPhongTornadoImpact");
        bladeOfVuImpact = Resources.Load<GameObject>("BladeOfVuImpact");
        arrow = Resources.Load<GameObject>("Arrow");
        elementalLeafRangerArrow = Resources.Load<GameObject>("ElementalLeafRangerArrow");
        elementalLeafRangerPoisonArrowImpact = Resources.Load<GameObject>(
            "ElementalLeafRangerPoisonArrowImpact"
        );
        elementalLeafRangerVineArrowImpact = Resources.Load<GameObject>(
            "ElementalLeafRangerVineArrowImpact"
        );
        wanderMagicianProjectile = Resources.Load<GameObject>("WanderMagicianProjectile");
        vanishEffect = Resources.Load<GameObject>("VanishEffect");
        bladeOfMinhKhaiSlashEffect = Resources.Load<GameObject>("BladeOfMinhKhaiSlashEffect");
        bladeOfPhongTornadoEffect = Resources.Load<GameObject>("BladeOfPhongTornadoEffect");
        ghost = Resources.Load<GameObject>("Ghost");
        bladeOfVuStar = Resources.Load<GameObject>("BladeOfVuStar");
        bladeOfVuSlash = Resources.Load<GameObject>("BladeOfVuSlash");
        pierceStrike = Resources.Load<GameObject>("PierceStrike");
        pierceStrikeSecondPhase = Resources.Load<GameObject>("PierceStrikeSecondPhase");
        deepBladeSlash = Resources.Load<GameObject>("DeepBladeSlash");
        rayOfJungleBeam = Resources.Load<GameObject>("RayOfJungleBeam");
        woodCryArrow = Resources.Load<GameObject>("WoodCryArrow");
        elementalLeafRangerPoisonArrow = Resources.Load<GameObject>(
            "ElementalLeafRangerPoisonArrow"
        );
        elementalLeafRangerVineArrow = Resources.Load<GameObject>("ElementalLeafRangerVineArrow");
        lightingForwardLighting = Resources.Load<GameObject>("LightingForwardLighting");
        blueHole = Resources.Load<GameObject>("BlueHole");
        nuclearBombExplosion = Resources.Load<GameObject>("NuclearBombExplosion");
        swordTempestSlash1 = Resources.Load<GameObject>("SwordTempestSlash1");
        swordTempestSlash2 = Resources.Load<GameObject>("SwordTempestSlash2");
        swordTempestSlash3 = Resources.Load<GameObject>("SwordTempestSlash3");
        dashEffect = Resources.Load<GameObject>("DashEffect");
        dashExplode = Resources.Load<GameObject>("DashExplode");
        magicLaser = Resources.Load<GameObject>("MagicLaser");
        moonSlash = Resources.Load<GameObject>("MoonSlash");
        scatterArrow = Resources.Load<GameObject>("ScatterArrow");
        scatterCharge = Resources.Load<GameObject>("ScatterCharge");
        attackCollider = Resources.Load<GameObject>("AttackCollider");
        dieDissolve = Resources.Load<GameObject>("DieDissolve");
        strongDudeShockwave = Resources.Load<GameObject>("StrongDudeShockwave");
        knockUpCollider = Resources.Load<GameObject>("KnockUpCollider");
        pushRandomCollider = Resources.Load<GameObject>("PushRandomCollider");
        pushCollider = Resources.Load<GameObject>("PushCollider");
        stunCollider = Resources.Load<GameObject>("StunCollider");
        phantomPulseDragon = Resources.Load<GameObject>("PhantomPulseDragon");
        infernalTideFlame = Resources.Load<GameObject>("InfernalTideFlame");
        infernalTideFlameNoReceiver = Resources.Load<GameObject>("InfernalTideFlameNoReceiver");
        infernalTideFan = Resources.Load<GameObject>("InfernalTideFan");
        scatterFlash = Resources.Load<GameObject>("ScatterFlash");
        stormFangMergeBlades = Resources.Load<GameObject>("StormFangMergeBlades");
        stormFangMergeProgress = Resources.Load<GameObject>("StormFangMergeProgress");
        kunai = Resources.Load<GameObject>("Kunai");
        kunaiHit = Resources.Load<GameObject>("KunaiHit");
        arcaneSwarmSlash1 = Resources.Load<GameObject>("ArcaneSwarmSlash1");
        arcaneSwarmSlash2 = Resources.Load<GameObject>("ArcaneSwarmSlash2");
        arcaneSwarmSlash3 = Resources.Load<GameObject>("ArcaneSwarmSlash3");
        arcaneSwarmSlash4 = Resources.Load<GameObject>("ArcaneSwarmSlash4");
        arcaneSwarmSlash5 = Resources.Load<GameObject>("ArcaneSwarmSlash5");
        orbitalNemesisDash = Resources.Load<GameObject>("OrbitalNemesisDash");
        heliosGazeRay = Resources.Load<GameObject>("HeliosGazeRay");
        rimuruCombo2SlashA = Resources.Load<GameObject>("RimuruCombo2SlashA");
        rimuruCombo2SlashB = Resources.Load<GameObject>("RimuruCombo2SlashB");
    }
}
