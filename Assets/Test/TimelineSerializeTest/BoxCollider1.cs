using UnityEngine;

public class BoxCollider1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Collider 1");
    }
}
