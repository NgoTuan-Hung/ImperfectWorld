using UnityEngine;

[DefaultExecutionOrder(-2)]
public class TestUIEventAndFixedUpdateOrder1 : MonoBehaviour
{
    public TestUIEventAndFixedUpdateOrderManager manager;

    private void Awake()
    {
        manager = FindAnyObjectByType<TestUIEventAndFixedUpdateOrderManager>();
    }

    private void FixedUpdate()
    {
        manager.test++;
    }
}
