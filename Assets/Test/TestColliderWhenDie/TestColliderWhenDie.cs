using UnityEngine;

public class TestColliderWhenDie : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("OntriggerEnter2D");
    }
}
