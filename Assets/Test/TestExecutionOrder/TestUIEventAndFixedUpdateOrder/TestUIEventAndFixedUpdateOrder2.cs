using UnityEngine;

[DefaultExecutionOrder(-1)]
public class TestUIEventAndFixedUpdateOrder2 : MonoBehaviour
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
