using System.Collections;
using System.Linq;
using UnityEngine;

public partial class GameManager
{
    public bool LoadNormalEnemyRoomVariant(int p_index)
    {
        /* GameUIManager.Instance.gameInteractionButton.Show();
        if (enemyCount > 0)
            return false;
        else
        {
            if (p_index < 0 || p_index >= roomSystem.allNormalEnemyRooms.Count)
                return false;

            ChangeGameState(GameState.PositioningPhase);
            GameUIManager.Instance.TurnOffMap();
            enemyCount = 0;
            var nERI = roomSystem.allNormalEnemyRooms[p_index];
            GetEnemyTeamChampions().Clear();

            for (int i = 0; i < nERI.roomEnemyInfos.Count; i++)
            {
                ObjectPool t_pool;
                if (!championPools.TryGetValue(nERI.roomEnemyInfos[i].prefab, out t_pool))
                {
                    championPools.Add(
                        nERI.roomEnemyInfos[i].prefab,
                        new ObjectPool(
                            nERI.roomEnemyInfos[i].prefab,
                            new PoolArgument(
                                ComponentType.CustomMono,
                                PoolArgument.WhereComponent.Self
                            )
                        )
                    );
                }

                var t_customMono = championPools[nERI.roomEnemyInfos[i].prefab]
                    .PickOne()
                    .CustomMono;
                t_customMono.transform.position = nERI.roomEnemyInfos[i].position;
                t_customMono.stat.currentHealthPointReachZeroEvent += PawnDeathHandler;
                MarkGridNodeAsEnemyNode(t_customMono.transform.position);
                GetEnemyTeamChampions().Add(t_customMono);
                enemyCount++;
            }

            GetEnemyTeamChampions()
                .ForEach(cRE =>
                {
                    cRE.botAIManager.aiBehavior.pausableScript.pauseFixedUpdate();
                    cRE.botSensor.pausableScript.pauseFixedUpdate();
                });
        } */

        return true;
    }

    public void LoadShopRoomDebug()
    {
        LoadShopRoom();
    }

    public bool SpawnChampionForPlayer(GameObject champPrefab)
    {
        if (RewardChampion(champPrefab) != null)
            return true;
        else
            return false;
    }

    public bool SpawnChampionForPlayerForBattle(GameObject champPrefab)
    {
        var t_customMono = RewardChampion(champPrefab);

        if (t_customMono == null)
            return false;

        StartCoroutine(EnableBattleModeIE(t_customMono));
        return true;
    }

    /// <summary>
    /// Wait one frame after CustomMono initialization
    /// </summary>
    /// <param name="customMono"></param>
    /// <returns></returns>
    IEnumerator EnableBattleModeIE(CustomMono customMono)
    {
        yield return null;
        EnableBattleMode(customMono);
    }

    public void TestRelic()
    {
        var relic = relicPool
            .PickOne()
            .Relic.Setup(relicDataSOs.First(r => r.name.Equals("BloodWingBlessing")));

        relic.transform.SetParent(GameUIManager.Instance.relicContent.transform, false);
    }

    public void TestItem()
    {
        var item = GetRandomItemRewards(1);
        GetPlayerTeamChampions()[0].stat.EquipItem(item[0]);
    }
}
