public class ActionLogic
{
    public BaseAction baseAction;

    public static void NoAction() { }

    public ActionLogic(BaseAction baseAction)
    {
        this.baseAction = baseAction;
    }
}
