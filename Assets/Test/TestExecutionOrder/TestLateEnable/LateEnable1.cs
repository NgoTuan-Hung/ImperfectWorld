using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class LateEnable1 : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        print("LateEnable1");
    }
}
