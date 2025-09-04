using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MethodAllocTest : MonoBehaviour
{
    class A
    {
        int x;
        double y = 9;
        double z;
        double j;

        public void M1()
        {
            x++;
        }

        public void M2()
        {
            x++;
        }
        // ... add more methods if you want
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = GC.GetTotalMemory(true);

        const int count = 100000;
        A[] array = new A[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = new A();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long after = GC.GetTotalMemory(true);

        long size = (after - before) / count;

        print($"Approximate size per instance: {size} bytes");
    }

    // Update is called once per frame
    void Update() { }
}
