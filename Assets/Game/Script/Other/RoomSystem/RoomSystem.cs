using System;
using System.Collections.Generic;

[Serializable]
public class RoomSystem
{
    public List<NormalEnemyRoomInfo> allNormalEnemyRooms = new();
    public List<NormalEnemyFloor> normalEnemyFloors = new();
}
