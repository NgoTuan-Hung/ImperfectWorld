using System.Collections;
using UnityEngine;

public class TestExecutionOrder2 : MonoBehaviour
{
    private void Awake()
    {
        print("2 AWAKE");
    }

    private void OnEnable()
    {
        print("2 ONENABLE");
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        print("2 LATEENABLE");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("2 START");
    }
}
