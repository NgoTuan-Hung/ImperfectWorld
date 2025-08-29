using System;
using System.Collections;
using UnityEngine;

public class ComboAction
{
    public Func<Vector2, Vector2, IEnumerator> actionMethod = NoActionIE;

    public ComboAction(Func<Vector2, Vector2, IEnumerator> actionMethod)
    {
        this.actionMethod = actionMethod;
    }

    public IEnumerator actionIE = NoActionIE(Vector2.zero, Vector2.zero);

    public ComboAction() { }

    static IEnumerator NoActionIE(Vector2 p_pos, Vector2 p_dir)
    {
        yield break;
    }
}
