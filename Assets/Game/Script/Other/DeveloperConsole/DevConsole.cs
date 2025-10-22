using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    string output = "";

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
}
