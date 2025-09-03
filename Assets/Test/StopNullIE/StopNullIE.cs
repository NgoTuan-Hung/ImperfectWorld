using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StopNullIE : MonoBehaviour
{
    IEnumerator stopNull;
    bool canPressA = true,
        canPressB = true;

    IEnumerator StopNull()
    {
        yield return new WaitForSeconds(1);
        print("StopNullIE");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.aKey.isPressed && canPressA)
        {
            StartCoroutine(ResetA());
            stopNull = StopNull();
            StartCoroutine(stopNull);
        }
        if (Keyboard.current.bKey.isPressed && canPressB)
        {
            StartCoroutine(ResetB());
            StopCoroutine(stopNull);
        }
    }

    IEnumerator ResetA()
    {
        canPressA = false;
        yield return new WaitForSeconds(0.1f);
        canPressA = true;
    }

    IEnumerator ResetB()
    {
        canPressB = false;
        yield return new WaitForSeconds(0.1f);
        canPressB = true;
    }
}
