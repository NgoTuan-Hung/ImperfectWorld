/// <summary>
/// Mostly be used by UI for showing cooldown, number of usages, etc.
/// </summary>
public class ActionResult
{
    public bool success = false;
    public bool timer = false;
    public float cooldown;

    public ActionResult(bool success, bool timer, float cooldown)
    {
        this.success = success;
        this.timer = timer;
        this.cooldown = cooldown;
    }

    public ActionResult() { }
}
