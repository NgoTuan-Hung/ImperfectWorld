using UnityEngine;

[DefaultExecutionOrder(2)]
public class TestExecutionOrder1 : MonoBehaviour
{
    private void Awake()
    {
        print("1 AWAKE");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    private void FixedUpdate()
    {
        print("1 FIXED UPDATE");
    }

    // Update is called once per frame
    void Update()
    {
        print("1 UPDATE");
    }
}
