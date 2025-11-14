using System;
using System.Collections.Generic;

[Serializable]
public class NormalEnemyRoomInfo
{
    public List<RoomEnemyInfo> roomEnemyInfos;

    public NormalEnemyRoomInfo()
    {
        roomEnemyInfos = new();
    }
}
