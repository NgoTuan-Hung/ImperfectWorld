using System.Collections.Generic;
using UnityEngine;

public class TestFindingChildCollider : MonoBehaviour
{
    BoxCollider2D boxCollider2D;
    CircleCollider2D circleCollider2D;
    List<Collider2D> collidersThisTrig = new();

    private void Awake()
    {
        boxCollider2D = transform.GetChild(0).GetComponent<BoxCollider2D>();
        circleCollider2D = transform.GetChild(1).GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetContacts(collidersThisTrig);

        foreach (var collider in collidersThisTrig)
        {
            if (collider == boxCollider2D)
                print("BoxCollider2D");
            else if (collider == circleCollider2D)
                print("CircleCollider2D");
        }
    }
}
