using UnityEngine;

[DefaultExecutionOrder(1)]
public class TestExecutionOrder2 : MonoBehaviour
{
    private void Awake()
    {
        print("2 AWAKE");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    private void FixedUpdate()
    {
        print("2 FIXED UPDATE");
    }

    // Update is called once per frame
    void Update()
    {
        print("2 UPDATE");
    }
}
