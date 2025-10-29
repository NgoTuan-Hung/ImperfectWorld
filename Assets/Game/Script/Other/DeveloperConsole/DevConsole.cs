using System.Collections.Generic;
using System.Linq;
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
        DebugLogConsole.AddCommand<bool>(
            "get-all-people",
            "Find all game objects with CustomMono component",
            GetAllPeople
        );

        DebugLogConsole.AddCommand<int>(
            "load-normal-enemy-room-variant",
            "Load Normal Enemy Room Variant",
            LoadNormalEnemyRoomVariant
        );

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
    }

    void GetAllPeople(bool p_includeInactive)
    {
        List<GameObject> t_people = FindObjectsByType<CustomMono>(
                p_includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            )
            .Select(cM => cM.gameObject)
            .ToList();

        output = "";
        foreach (GameObject t in t_people)
            output += t.name + "-" + $"<color=#00FF00>{t.GetHashCode()}</color>" + "\n";

        Debug.Log(output);
    }

    void LoadNormalEnemyRoomVariant(int p_index)
    {
        Debug.Log(
            "Load Normal Enemy Room Variant: "
                + GameManager.Instance.LoadNormalEnemyRoomVariant(p_index)
        );
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
}
