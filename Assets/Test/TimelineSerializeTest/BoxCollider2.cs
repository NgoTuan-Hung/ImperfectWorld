using UnityEngine;

public class BoxCollider2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Collider 2");
    }
}
