using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Order script executions in a managable way.
/// </summary>
public class ScriptExecutionCore : MonoBehaviour
{
    public Action lateAwake0 = () => { },
        lateEnable0 = () => { },
        lateEnableOnce = () => { };

    private void Awake()
    {
        StartCoroutine(LateAwake());
    }

    IEnumerator LateAwake()
    {
        yield return null;
        lateAwake0();
    }

    private void OnEnable()
    {
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        lateEnable0();
        lateEnableOnce();
        lateEnableOnce = () => { };
    }

    private void Start()
    {
        //
    }
}
