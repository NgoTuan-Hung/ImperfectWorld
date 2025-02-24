using System;
using UnityEngine;

public static class VectorExtension
{
	public static Vector2 RotateZ(this Vector2 vector2, float degrees)
    {
    	return new Vector2((float)(Math.Cos(degrees) * vector2.x - Math.Sin(degrees) * vector2.y), (float)(Math.Sin(degrees) * vector2.x + Math.Cos(degrees) * vector2.y));
    }
}