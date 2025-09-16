using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnEnemyInfo
{
    public GameObject prefab;
    public List<SpawnEnemySkillInfo> spawnEnemySkillInfos = new();

    public void Init()
    {
        var t_skill = prefab.GetComponent<Skill>();
        t_skill.skillDataSOs.ForEach(sDO =>
        {
            if (!sDO.skillName.Equals("Attackable"))
                spawnEnemySkillInfos.Add(new(sDO.skillName));
        });
    }

    /// <summary>
    /// Unlock any available next skill, if none available, return false
    /// </summary>
    /// <returns></returns>
    public bool UnlockNextSkill()
    {
        for (int i = 0; i < spawnEnemySkillInfos.Count; i++)
        {
            if (!spawnEnemySkillInfos[i].unlocked)
            {
                spawnEnemySkillInfos[i].unlocked = true;
                return true;
            }
        }

        return false;
    }
}
