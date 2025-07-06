using UnityEngine;

public class MultipleColliderOnOneObjectTest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("OnTriggerEnter2D");
    }
}
