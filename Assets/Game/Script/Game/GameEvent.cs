using System;

public class GameEvent
{
    public Action<IGameEventData> action = (p_gED) => { };
}
