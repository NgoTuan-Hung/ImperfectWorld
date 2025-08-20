using System.Collections.Generic;
using UnityEngine;

public class TestCulmulativeDistribution : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<float> l1 = new() { 0.3f, 0.4f, 0.6f, 1 };
        print(
            "{ 0.3f, 0.4f, 0.6f, 1 } retrieve 0.2f: "
                + l1.CumulativeDistributionBinarySearch(0, l1.Count - 1, 0.2f)
        );
        print(
            "{ 0.3f, 0.4f, 0.6f, 1 } retrieve 0.3f: "
                + l1.CumulativeDistributionBinarySearch(0, l1.Count - 1, 0.3f)
        );
        print(
            "{ 0.3f, 0.4f, 0.6f, 1 } retrieve 0.7f: "
                + l1.CumulativeDistributionBinarySearch(0, l1.Count - 1, 0.7f)
        );
        print(
            "{ 0.3f, 0.4f, 0.6f, 1 } retrieve 1f: "
                + l1.CumulativeDistributionBinarySearch(0, l1.Count - 1, 1f)
        );
        List<float> l2 = new() { 0, 0, 1, 1, 1 };
        print(
            "{ 0, 0, 1, 1, 1 } retrieve 0.5f: "
                + l2.CumulativeDistributionBinarySearch(0, l2.Count - 1, 0.5f)
        );
        print(
            "{ 0, 0, 1, 1, 1 } retrieve 0f: "
                + l2.CumulativeDistributionBinarySearch(0, l2.Count - 1, 0f)
        );

        B b = new B();
        b.A1();
    }

    // Update is called once per frame
    void Update() { }

    public class A
    {
        public virtual void A1()
        {
            A2();
        }

        public virtual void A2()
        {
            print("A-A2");
        }
    }

    public class B : A
    {
        public override void A2()
        {
            print("B-A2");
        }
    }
}
