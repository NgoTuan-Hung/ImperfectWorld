using System;
using System.Collections.Generic;
using System.Diagnostics;
using Priority_Queue;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GridManager : MonoBehaviour
{
    public int xMin,
        yMin,
        xMax,
        yMax;
    public float nodeSize = 1f;
    public List<List<GridNode>> gridNodes = new();
    GridNode border = new(Vector2.zero);
    SimplePriorityQueue<GridNode> queue = new();
    Dictionary<GridNode, GridNode> cameFrom = new();
    Dictionary<GridNode, float> costSoFar = new();
    Action resetGrid = () => { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        border.type = GridNodeType.Obstacle;
        var width = (int)((xMax - xMin) / nodeSize);
        var height = (int)((yMax - yMin) / nodeSize);
        var nodeOffset = nodeSize / 2f;

        for (int i = 0; i < width; i++)
        {
            gridNodes.Add(new List<GridNode>());
            for (int j = 0; j < height; j++)
            {
                gridNodes[i]
                    .Add(
                        new(new(xMin + nodeSize * i + nodeOffset, yMax - nodeSize * j - nodeOffset))
                    );

                resetGrid += gridNodes[i][j].Reset;
            }
        }

        for (int i = 0; i < gridNodes.Count; i++)
        {
            for (int j = 0; j < gridNodes[i].Count; j++)
            {
                for (int x = i - 1; x < i + 2; x++)
                {
                    for (int y = j - 1; y < j + 2; y++)
                    {
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            if (x != i || y != j)
                            {
                                gridNodes[i][j].neighbors.Add(gridNodes[x][y]);
                            }
                        }
                        else
                            gridNodes[i][j].neighbors.Add(border);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        ResetGrid();
    }

    public void ResetGrid()
    {
        resetGrid();
    }

    public List<GridNode> SolvePath(GridNode start, GridNode end)
    {
        queue = new();
        cameFrom = new();
        costSoFar = new();
        queue.Enqueue(start, 0);
        cameFrom.Add(start, null);
        costSoFar.Add(start, 0);
        var path = new List<GridNode>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
            {
                path.Add(current);
                while (cameFrom[current] != null)
                {
                    current = cameFrom[current];
                    path.Add(current);
                }
                return path;
            }

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor.type == GridNodeType.Obstacle)
                {
                    continue;
                }

                var newCost = costSoFar[current] + Vector2.Distance(current.pos, neighbor.pos);
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    var priority = newCost + Vector2.Distance(neighbor.pos, end.pos);
                    if (!queue.Contains(neighbor))
                        queue.Enqueue(neighbor, priority);
                    else
                        queue.UpdatePriority(neighbor, priority);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return path;
    }

    /// <summary>
    /// GPT GENERATED
    /// </summary>
    /// <returns></returns>
    public GridNode GetNodeAtPosition(Vector2 pos)
    {
        // Step 2: Convert the world position to grid coordinates
        int gridX = Mathf.FloorToInt((pos.x - xMin) / nodeSize);
        int gridY = Mathf.FloorToInt((yMax - pos.y) / nodeSize);

        // Step 3: Check if the position is within bounds of the grid
        if (gridX >= 0 && gridX < gridNodes.Count && gridY >= 0 && gridY < gridNodes[gridX].Count)
        {
            return gridNodes[gridX][gridY];
        }
        else
        {
            // If the mouse is out of bounds, return the border node
            return border;
        }
    }

    /// <summary>
    /// GPT GENERATED
    /// </summary>
    /// <param name="centerPos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void MarkObstacleSquare(Vector2 centerPos, float width, float height)
    {
        // Convert centerPos from world coordinates to grid coordinates
        int centerX = Mathf.FloorToInt((centerPos.x - xMin) / nodeSize);
        int centerY = Mathf.FloorToInt((yMax - centerPos.y) / nodeSize);

        // Calculate half-width and half-height to define the bounds
        int halfWidth = Mathf.FloorToInt(width / 2f / nodeSize);
        int halfHeight = Mathf.FloorToInt(height / 2f / nodeSize);

        // Iterate through all nodes within the bounds of the square
        for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
        {
            for (int y = centerY - halfHeight; y <= centerY + halfHeight; y++)
            {
                // Check if the node is within bounds of the grid
                if (x >= 0 && x < gridNodes.Count && y >= 0 && y < gridNodes[x].Count)
                {
                    gridNodes[x][y].type = GridNodeType.Obstacle; // Mark this node as an obstacle
                }
            }
        }
    }

    /// <summary>
    /// GPT GENERATED
    /// </summary>
    /// <param name="centerPos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void RemoveObstacleSquare(Vector2 centerPos, float width, float height)
    {
        // Convert centerPos from world coordinates to grid coordinates
        int centerX = Mathf.FloorToInt((centerPos.x - xMin) / nodeSize);
        int centerY = Mathf.FloorToInt((yMax - centerPos.y) / nodeSize);

        // Calculate half-width and half-height to define the bounds
        int halfWidth = Mathf.FloorToInt(width / 2f / nodeSize);
        int halfHeight = Mathf.FloorToInt(height / 2f / nodeSize);

        // Iterate through all nodes within the bounds of the square
        for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
        {
            for (int y = centerY - halfHeight; y <= centerY + halfHeight; y++)
            {
                // Check if the node is within bounds of the grid
                if (x >= 0 && x < gridNodes.Count && y >= 0 && y < gridNodes[x].Count)
                {
                    gridNodes[x][y].type = GridNodeType.Normal;
                }
            }
        }
    }
}
