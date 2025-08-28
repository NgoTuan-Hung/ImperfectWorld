using System;
using System.Collections;
using UnityEngine;

public class ComboAction
{
    public Action<Vector2, Vector2> action = NoAction;
    public IEnumerator iEnumerator;

    public ComboAction() { }

    public ComboAction(Action<Vector2, Vector2> action, IEnumerator iEnumerator)
    {
        this.action = action;
        this.iEnumerator = iEnumerator;
    }

    static void NoAction(Vector2 p_pos, Vector2 p_dir) { }
}
