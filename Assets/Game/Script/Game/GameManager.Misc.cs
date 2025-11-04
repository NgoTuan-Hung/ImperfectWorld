using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    public Material team1DirectionIndicatorMat;
    public Material team2DirectionIndicatorMat;
    public Material damagePopupMat,
        weakenPopupMat,
        armorBuffPopupMat;
    Dictionary<string, string> descriptionDB = new()
    {
        { "hp", "<link=hp><color=#C71F37>hp</color></link>: health point." },
        {
            "hp regen",
            "<link=hp regen><color=#FF6B6B>hp regen</color></link>: how many <link=hp><color=#C71F37>hp</color></link> is regenerated per second."
        },
        {
            "current mp",
            "<link=current mp><color=#3E8EDE>current mp</color></link>: current mana point."
        },
        { "mp", "<link=mp><color=#2F75C0>mp</color></link>: mana point." },
        {
            "mp regen",
            "<link=mp regen><color=#7FDBFF>mp regen</color></link>: how many <link=hp><color=#C71F37>hp</color></link> is regenerated per second."
        },
        {
            "might",
            "<link=might><color=#F39C12>might</color></link>: increases <link=hp><color=#C71F37>hp</color></link> and <link=hp regen><color=#FF6B6B>hp regen</color></link>."
        },
        {
            "reflex",
            "<link=reflex><color=#27AE60>reflex</color></link>: increases <link=armor><color=#95A5A6>armor</color></link> and <link=aspd><color=#E67E22>aspd</color></link>."
        },
        {
            "wisdom",
            "<link=wisdom><color=#9B59B6>wisdom</color></link>: increases <link=mp><color=#2F75C0>mp</color></link> and <link=mp regen><color=#7FDBFF>mp regen</color></link>."
        },
        { "aspd", "<link=aspd><color=#E67E22>aspd</color></link>: attack speed." },
        { "armor", "<link=armor><color=#95A5A6>armor</color></link>: 1 armor mitigates 1 damage." },
        {
            "omnivamp",
            "<link=omnivamp><color=#C62828>omnivamp</color></link>: heals from damage dealt."
        },
        { "atk", "<link=atk><color=#E53935>atk</color></link>: attack damage." },
        {
            "damage reduction",
            "<link=damage reduction><color=#4A90E2>damage reduction</color></link>: reduces total damage taken, applied after armor."
        },
        {
            "strike lock",
            "<link=strike lock><color=#fc03d7>strike lock</color></link>: one affected by strike lock cannot attack."
        },
    };
}
