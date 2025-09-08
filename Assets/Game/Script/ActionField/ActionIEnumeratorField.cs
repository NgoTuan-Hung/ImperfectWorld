using System.Collections;

public class ActionIEnumeratorField : ActionField
{
    public IEnumerator value = NoIE();

    public static IEnumerator NoIE()
    {
        yield break;
    }
}
