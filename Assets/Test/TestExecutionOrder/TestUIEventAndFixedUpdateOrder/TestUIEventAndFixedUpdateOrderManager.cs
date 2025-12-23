using UnityEngine;

[DefaultExecutionOrder(0)]
public class TestUIEventAndFixedUpdateOrderManager : MonoBehaviour
{
    public int test = 0;

    private void FixedUpdate()
    {
        test = 0;
    }
}
