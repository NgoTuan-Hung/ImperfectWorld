using System;

public static class FloatExtension
{
	public static float DegToRad(this float deg) => (float)Math.PI * deg / 180;
}