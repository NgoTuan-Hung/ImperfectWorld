using UnityEngine;

public static class TransformExtension
{
    public static void AddPos(this Transform p_transform, Vector2 p_vector2)
    {
        p_transform.position += (Vector3)p_vector2;
    }

    public static void RotateToDirection(this Transform p_transform, Vector2 p_direction)
    {
        p_transform.rotation = Quaternion.Euler(
            p_transform.rotation.eulerAngles.WithZ(Vector2.SignedAngle(Vector2.right, p_direction))
        );
    }

    public static void SetEulerZ(this Transform p_transform, float z)
    {
        p_transform.rotation = Quaternion.Euler(p_transform.rotation.eulerAngles.WithZ(z));
    }
}
