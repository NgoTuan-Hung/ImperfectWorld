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
            "unlock-all-skill",
            "Unlock all skills for a CustomMono",
            UnlockAllSkillFor
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

    void UnlockAllSkillFor(int p_hashCode)
    {
        CustomMono t_customMono = FindObjectsByType<CustomMono>(FindObjectsSortMode.None)
            .FirstOrDefault(cM => cM.gameObject.GetHashCode() == p_hashCode);

        if (t_customMono != null)
        {
            t_customMono.skill.skillBases.ForEach(sB => t_customMono.skill.UnlockSkill(sB));

            Debug.Log($"<color=#00FF00>Unlocked</color>");
        }
        else
            Debug.Log($"<color=#fc0303>Invalid HashCode</color>");
    }
}
