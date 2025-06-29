using UnityEngine;

public class ColliderCopier : MonoBehaviour
{
    PolygonCollider2D polygonCollider2D;
    public ColliderCopier other;
    public bool copy = false;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        if (copy)
        {
            polygonCollider2D.points = other.polygonCollider2D.points;
            polygonCollider2D.offset = other.polygonCollider2D.offset;
        }
    }
}
