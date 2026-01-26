using System;
using System.Collections.Generic;

[Serializable]
public class EnemyRoomInfo
{
    public List<RoomEnemyInfo> roomEnemyInfos;

    public EnemyRoomInfo()
    {
        roomEnemyInfos = new();
    }
}
