using System.Collections.Generic;
using System.Linq;
using System.Text;
using IngameDebugConsole;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    string output = "";
    List<GameObject> availableChampions;

    private void Awake()
    {
        availableChampions = Resources.LoadAll<GameObject>("Champion/").ToList();
    }

    private void Start()
    {
        DebugLogConsole.AddCommand(
            "get-all-battlers",
            "Find all game objects with CustomMono component",
            GetAllBattlers
        );

        DebugLogConsole.AddCommand<int>(
            "load-normal-enemy-room-variant",
            "Load Normal Enemy Room Variant",
            LoadNormalEnemyRoomVariant
        );

        DebugLogConsole.AddCommand(
            "load-mystery-event-room",
            "Load Mystery Event Room",
            LoadMysteryEventRoom
        );

        DebugLogConsole.AddCommand("load-shop-room", "Load Shop Room", LoadShopRoom);

        DebugLogConsole.AddCommand(
            "get-all-available-champion",
            "Get all available champions",
            GetAllAvailableChampion
        );

        DebugLogConsole.AddCommand<int>(
            "spawn-champion-for-player",
            "Spawn a champion for player",
            SpawnChampionForPlayer
        );

        DebugLogConsole.AddCommand<int>(
            "spawn-champion-for-player-for-battle",
            "Spawn a champion for player for battle",
            SpawnChampionForPlayerForBattle
        );

        DebugLogConsole.AddCommand(
            "get-all-stat-upgrades",
            "Get all stat upgrades",
            GetAllStatUpgrades
        );

        DebugLogConsole.AddCommand<int, int>(
            "upgrade-stat-for",
            "Upgrade stat for battler",
            UpgradeStatFor
        );

        DebugLogConsole.AddCommand("test-relic", "Test relic", TestRelic);
        DebugLogConsole.AddCommand("test-item", "Test item", TestItem);
    }

    void GetAllBattlers()
    {
        StringBuilder sb = new();
        var battlers = FindObjectsByType<CustomMono>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        for (int i = 0; i < battlers.Length; i++)
            sb.AppendLine(battlers[i].name + ": " + $"<color=#00FF00>{i}</color>");

        Debug.Log(sb.ToString());
    }

    void LoadNormalEnemyRoomVariant(int p_index)
    {
        Debug.Log(
            "Load Normal Enemy Room Variant: "
                + GameManager.Instance.LoadNormalEnemyRoomVariant(p_index)
        );
    }

    void LoadMysteryEventRoom()
    {
        Debug.Log("Loaded Mystery Event Room !!");
        GameManager.Instance.LoadMysteryEventRoom();
    }

    void LoadShopRoom()
    {
        GameManager.Instance.LoadShopRoomDebug();
    }

    void GetAllAvailableChampion()
    {
        output = "";

        for (int i = 0; i < availableChampions.Count; i++)
        {
            output += availableChampions[i].name + "-" + $"<color=#00FF00>{i}</color>" + "\n";
        }

        Debug.Log(output);
    }

    void SpawnChampionForPlayer(int p_champIndex)
    {
        Debug.Log(
            "Spawn Champion For Player: "
                + GameManager.Instance.SpawnChampionForPlayer(availableChampions[p_champIndex])
        );
    }

    void SpawnChampionForPlayerForBattle(int p_champIndex)
    {
        Debug.Log(
            "Spawn Champion For Player: "
                + GameManager.Instance.SpawnChampionForPlayerForBattle(
                    availableChampions[p_champIndex]
                )
        );
    }

    void GetAllStatUpgrades()
    {
        StringBuilder sb = new();
        List<StatUpgrade> statBuffs = Resources
            .LoadAll<StatUpgrade>("ScriptableObject/StatUpgrade/")
            .ToList();
        for (int i = 0; i < statBuffs.Count; i++)
        {
            sb.AppendLine($"{i}: {statBuffs[i].description}");
        }

        Debug.Log(sb.ToString());
    }

    void UpgradeStatFor(int battlerID, int statUpgradeID)
    {
        var statUpgrades = Resources.LoadAll<StatUpgrade>($"ScriptableObject/StatUpgrade");
        if (statUpgradeID >= statUpgrades.Length)
        {
            Debug.LogError("Invalid stat upgrade ID: " + statUpgradeID);
            return;
        }

        var battlers = FindObjectsByType<CustomMono>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (battlerID < battlers.Length)
        {
            GameManager.Instance.UpgradeStat(battlers[battlerID], statUpgrades[statUpgradeID]);

            Debug.Log(
                $"Upgraded stat for {battlers[battlerID].name} with {statUpgrades[statUpgradeID].description}"
            );
        }
        else
            Debug.LogError("Invalid battler ID: " + battlerID);
    }

    void TestRelic()
    {
        GameManager.Instance.TestRelic();
    }

    void TestItem() => GameManager.Instance.TestItem();
}
