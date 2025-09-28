using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public enum GridNodeType
{
    Normal,
    Obstacle,
}

public class GridNode : FastPriorityQueueNode
{
    public GridNodeType type = GridNodeType.Normal;
    public Vector2 pos;
    public List<GridNode> neighbors = new();
    public GridNode cameFrom = null;
    public float costSoFar = 0;
    public bool visited = false;

    public GridNode(Vector2 pos)
    {
        this.pos = pos;
    }

    public void Reset() => type = GridNodeType.Normal;

    public void ResetForPathFinding()
    {
        visited = false;
    }
}
