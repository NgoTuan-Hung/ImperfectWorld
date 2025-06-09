using System;
using UnityEngine;

public class PropTest1
{
    public float f1 = 0;
}

public class ActionTester : MonoBehaviour
{
    public ClassTest1 c1;
    Action changeTest1 = () => { };

    private void Awake()
    {
        changeTest1 = () =>
        {
            c1.p1 = new PropTest1() { f1 = 1000 };
            print(c1.p1.f1);
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changeTest1();
    }

    // Update is called once per frame
    void Update() { }
}
