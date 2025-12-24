using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class LateEnable2 : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        print("LateEnable2");
    }
}
