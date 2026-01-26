using System;
using System.Collections.Generic;

[Serializable]
public class RoomSystem
{
    public List<EnemyFloor> normalEnemyFloors = new();
    public List<EnemyRoomInfo> eliteEnemyRoomInfos = new();
    public List<EnemyRoomInfo> bossRoomInfos = new();
}
