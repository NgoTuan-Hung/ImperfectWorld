using System.Collections;
using UnityEngine;

public class TestExecutionOrder1 : MonoBehaviour
{
    private void Awake()
    {
        print("1 AWAKE");
    }

    private void OnEnable()
    {
        print("1 ONENABLE");
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        print("1 LATEENABLE");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("1 START");
    }
}
