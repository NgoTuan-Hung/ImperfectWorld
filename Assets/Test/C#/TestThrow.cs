using System;
using UnityEngine;

public class TestThrow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OuterMethod();
    }

    // Update is called once per frame
    void Update() { }

    void OuterMethod()
    {
        ErrorHere();
    }

    void ErrorHere()
    {
        try
        {
            InnerMethod();
        }
        catch (System.Exception ex)
        {
            throw;
        }
    }

    void InnerMethod()
    {
        try
        {
            GameObject gameObject = null;
            gameObject.name = "ok";
        }
        catch (System.Exception)
        {
            throw new InvalidOperationException("Some thing is wrong in InnerMethod");
        }
    }
}
