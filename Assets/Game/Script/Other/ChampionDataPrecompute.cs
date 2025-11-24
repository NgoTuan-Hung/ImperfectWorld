using System;
using System.Text;

public class ChampionDataPrecompute
{
    public float offerHealthPoint;

    public float offerHealthRegen;

    public float offerManaPoint;

    public float offerManaRegen;

    public float offerMight;

    public float offerReflex;

    public float offerWisdom;

    public float offerAttackSpeed;

    public float offerArmor;

    public float offerMoveSpeed;

    public float offerDamageModifier;

    public float offerOmnivamp;

    public float offerAttackDamage;

    public float offerCritChance;

    public float offerCritDamageModifier;

    public float offerDamageReduction;

    public float offerAttackRange;

    public string statDescription,
        offerDescription;

    public ChampionDataPrecompute(ChampionData championData)
    {
        offerHealthPoint = championData.healthPoint * 0.25f;
        offerHealthRegen = championData.healthRegen * 0.25f;
        offerManaPoint = -championData.manaPoint * 0.05f;
        offerManaRegen = championData.manaRegen * 0.25f;
        offerMight = championData.might * 0.25f;
        offerReflex = championData.reflex * 0.25f;
        offerWisdom = championData.wisdom * 0.25f;
        offerAttackSpeed = championData.attackSpeed * 0.25f;
        offerArmor = championData.armor * 0.25f;
        offerMoveSpeed = championData.moveSpeed * 0.25f;
        offerDamageModifier = championData.damageModifier * 0.25f;
        offerOmnivamp = championData.omnivamp * 0.25f;
        offerAttackDamage = championData.attackDamage * 0.25f;
        offerCritChance = championData.critChance * 0.25f;
        offerCritDamageModifier = championData.critDamageModifier * 0.25f;
        offerDamageReduction = championData.damageReduction * 0.25f;
        offerAttackRange = championData.attackRange * 0.05f;

        ConstructStatDesc(championData);
        ConstructOfferDesc(championData);
    }

    private void ConstructOfferDesc(ChampionData championData)
    {
        StringBuilder sb = new();

        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.HP, $": {offerHealthPoint}"));
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.HPREGEN, $": {offerHealthRegen}")
        );
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.MP, $": {offerManaPoint}"));
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.MPREGEN, $": {offerManaRegen}")
        );
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.MIGHT, $": {offerMight}"));
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.REFLEX, $": {offerReflex}"));
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.WISDOM, $": {offerWisdom}"));
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.ASPD, $": {offerAttackSpeed}")
        );
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.ARMOR, $": {offerArmor}"));
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.MSPD, $": {offerMoveSpeed}"));
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.DMGMOD, $": {offerDamageModifier}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.OMNIVAMP, $": {offerOmnivamp}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.ATK, $": {offerAttackDamage}")
        );
        sb.AppendLine(GameManager.ConstructComplexText(ComplexTextID.CRIT, $": {offerCritChance}"));
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.CRITMOD, $": {offerCritDamageModifier}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.DMGREDUC, $": {offerDamageReduction}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.ATKRANGE, $": {offerAttackRange}")
        );

        offerDescription = sb.ToString();
    }

    private void ConstructStatDesc(ChampionData championData)
    {
        StringBuilder sb = new();

        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.HP, $": {championData.healthPoint}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.HPREGEN, $": {championData.healthRegen}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.MP, $": {championData.manaPoint}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.MPREGEN, $": {championData.manaRegen}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.MIGHT, $": {championData.might}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.REFLEX, $": {championData.reflex}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.WISDOM, $": {championData.wisdom}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.ASPD, $": {championData.attackSpeed}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.ARMOR, $": {championData.armor}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.MSPD, $": {championData.moveSpeed}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(
                ComplexTextID.DMGMOD,
                $": {championData.damageModifier}"
            )
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.OMNIVAMP, $": {championData.omnivamp}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.ATK, $": {championData.attackDamage}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(ComplexTextID.CRIT, $": {championData.critChance}")
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(
                ComplexTextID.CRITMOD,
                $": {championData.critDamageModifier}"
            )
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(
                ComplexTextID.DMGREDUC,
                $": {championData.damageReduction}"
            )
        );
        sb.AppendLine(
            GameManager.ConstructComplexText(
                ComplexTextID.ATKRANGE,
                $": {championData.attackRange}"
            )
        );

        statDescription = sb.ToString();
    }
}
