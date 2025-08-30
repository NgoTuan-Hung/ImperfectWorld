using System;
using System.Collections;
using UnityEngine;

public class RefTest : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new(1);
    bool b = false;

    IEnumerator ARef(Func<bool> p_b)
    {
        yield return _waitForSeconds1;
        Debug.Log(p_b());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ARef(PB));
        b = true;
    }

    bool PB() => b;
}
