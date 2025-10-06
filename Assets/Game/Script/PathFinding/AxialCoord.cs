using System;
using UnityEngine;

public class AxialCoord : IEquatable<AxialCoord>
{
    Vector2Int coord;
    public int Q => coord.x;
    public int R => coord.y;

    public AxialCoord(int q, int r)
    {
        coord = new Vector2Int(q, r);
    }

    public bool Equals(AxialCoord other)
    {
        if (other == null)
            return false;
        return coord == other.coord;
    }

    public override bool Equals(object obj)
    {
        return obj is AxialCoord other && Equals(other);
    }

    public override int GetHashCode()
    {
        return coord.GetHashCode();
    }

    public override string ToString()
    {
        return $"({Q}, {R})";
    }
}
