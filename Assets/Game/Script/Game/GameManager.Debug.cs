using System.Collections;
using UnityEngine;

public partial class GameManager
{
    public bool LoadNormalEnemyRoomVariant(int p_index)
    {
        GameUIManager.Instance.startBattleButton.Show();
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
        }

        return true;
    }

    public bool SpawnChampionForPlayer(GameObject champPrefab)
    {
        if (champPrefab == null)
            return false;

        if (!championPools.TryGetValue(champPrefab, out ObjectPool t_pool))
        {
            championPools.Add(
                champPrefab,
                new ObjectPool(
                    champPrefab,
                    new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                )
            );

            t_pool = championPools[champPrefab];
        }

        var t_customMono = t_pool.PickOne().CustomMono;
        t_customMono.transform.position = new Vector3(0, 0, 0);
        SwithTeam(t_customMono, "Team1");
        DisableBattleMode(t_customMono);
        return true;
    }

    public bool SpawnChampionForPlayerForBattle(GameObject champPrefab)
    {
        if (champPrefab == null)
            return false;

        if (!championPools.TryGetValue(champPrefab, out ObjectPool t_pool))
        {
            championPools.Add(
                champPrefab,
                new ObjectPool(
                    champPrefab,
                    new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                )
            );

            t_pool = championPools[champPrefab];
        }

        var t_customMono = t_pool.PickOne().CustomMono;
        t_customMono.transform.position = new Vector3(0, 0, 0);
        SwithTeam(t_customMono, "Team1");
        StartCoroutine(EnableBattleModeIE(t_customMono));
        return true;
    }

    IEnumerator EnableBattleModeIE(CustomMono customMono)
    {
        yield return null;
        EnableBattleMode(customMono);
    }
}
