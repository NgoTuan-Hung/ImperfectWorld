using UnityEngine;

public static class TransformExtension
{
    public static void AddPos(this Transform p_transform, Vector2 p_vector2)
    {
        p_transform.position += (Vector3)p_vector2;
    }
}
