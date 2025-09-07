using System;
using UnityEngine;

public static class VectorExtension
{
    public static Vector3 veryFar = new(9999, 0, 0);
    public static Vector2 inverseY = new(1, -1);

    public static Vector2 RotateZ(this Vector2 vector2, float degrees)
    {
        return new Vector2(
            (float)(Math.Cos(degrees) * vector2.x - Math.Sin(degrees) * vector2.y),
            (float)(Math.Sin(degrees) * vector2.x + Math.Cos(degrees) * vector2.y)
        );
    }

    public static Vector2 WithY(this Vector2 vector2, float y)
    {
        return new Vector2(vector2.x, y);
    }

    public static Vector2 WithX(this Vector2 vector2, float x)
    {
        return new Vector2(x, vector2.y);
    }

    public static Vector2 AsVector2(this Vector3 vector3) => (Vector2)vector3;

    public static Vector3 WithY(this Vector3 vector3, float y)
    {
        return new Vector3(vector3.x, y, vector3.z);
    }

    public static Vector3 WithX(this Vector3 vector3, float x)
    {
        return new Vector3(x, vector3.y, vector3.z);
    }

    public static Vector3 WithZ(this Vector3 vector3, float z)
    {
        return new Vector3(vector3.x, vector3.y, z);
    }

    public static Vector3 WithZ(this Vector2 vector2, float z)
    {
        return new Vector3(vector2.x, vector2.y, z);
    }

    public static Vector3 AsVector3(this Vector2 vector2) => (Vector3)vector2;

    public static Vector3 Mul(this Vector3 source, Vector3 dest) =>
        new(source.x * dest.x, source.y * dest.y, source.z * dest.z);

    public static Vector4 WithW(this Vector4 vector, float w) =>
        new Vector4(vector.x, vector.y, vector.z, w);
}
