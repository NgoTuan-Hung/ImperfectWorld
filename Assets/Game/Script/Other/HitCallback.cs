using System;

public class HitCallback
{
    public int count = 0;
    public CustomMono target = null;
    public Action<HitCallback> callback = (hC) => { };
}
