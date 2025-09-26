using System.Collections.Generic;
using UnityEngine;

public enum GridNodeType
{
    Normal,
    Obstacle,
}

public class GridNode
{
    public GridNodeType type = GridNodeType.Normal;
    public Vector2 pos;
    public List<GridNode> neighbors = new();

    public GridNode(Vector2 pos)
    {
        this.pos = pos;
    }

    public void Reset() => type = GridNodeType.Normal;
}
