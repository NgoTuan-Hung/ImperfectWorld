using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CharAttackInfo",
    menuName = "ScriptableObjects/CharAttackInfo",
    order = 0
)]
public class CharAttackInfo : ScriptableObject
{
    public enum AttackType
    {
        Melee,
        Ranged,
    }

    public AttackType attackType = AttackType.Melee;

    [Serializable]
    public class MeleeAttackInfo
    {
        public GameEffectSO meleeAttackEffectSO,
            meleeImpactEffectSO;
    }

    public List<MeleeAttackInfo> meleeAttackInfos = new();
    public int variant;
    public GameEffectSO rangedProjectileEffectSO,
        rangedImpactEffectSO;

    public GameEffect GetMeleeAttackEffect(int p_variant) =>
        GameManager
            .Instance.poolLink[meleeAttackInfos[p_variant].meleeAttackEffectSO]
            .PickOneGameEffect();

    public GameEffect GetMeleeImpactEffect(int p_variant) =>
        GameManager
            .Instance.poolLink[meleeAttackInfos[p_variant].meleeImpactEffectSO]
            .PickOneGameEffect();

    public bool CheckMeleeAttackEffect(int p_variant) =>
        meleeAttackInfos[p_variant].meleeAttackEffectSO != null;

    public bool CheckMeleeImpactEffect(int p_variant) =>
        meleeAttackInfos[p_variant].meleeImpactEffectSO != null;

    public GameEffect GetRangedProjectileEffect() =>
        GameManager.Instance.poolLink[rangedProjectileEffectSO].PickOneGameEffect();
}
