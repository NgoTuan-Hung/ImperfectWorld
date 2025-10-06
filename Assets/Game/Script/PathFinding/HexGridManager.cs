using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class HexGridManager : MonoBehaviour
{
    public GameObject debugPrefab;
    public int iteration = 3;
    Dictionary<AxialCoord, HexGridNode> grid = new();
    HashSet<HexGridNode> visited = new();
    int[][] neighborOffsets = new int[][]
    {
        new int[] { 1, 0 },
        new int[] { 1, -1 },
        new int[] { 0, -1 },
        new int[] { -1, 0 },
        new int[] { -1, 1 },
        new int[] { 0, 1 },
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HexGridNode origin = new(new AxialCoord(0, 0));
        grid.Add(origin.axialCoord, origin);

        Recursive(origin, 0);
        foreach (var kvp in grid)
        {
            print(kvp.Value.neighbors.Count);
        }
    }

    void GenerateNeighbors(HexGridNode parent)
    {
        for (int i = 0; i < 6; i++)
        {
            var axialCoord = new AxialCoord(
                parent.axialCoord.Q + neighborOffsets[i][0],
                parent.axialCoord.R + neighborOffsets[i][1]
            );
            if (!grid.ContainsKey(axialCoord))
            {
                grid.Add(axialCoord, new HexGridNode(axialCoord));
                var debug = Instantiate(debugPrefab, grid[axialCoord].pos, Quaternion.identity);
                debug.name = axialCoord.ToString();
            }
            parent.neighbors.Add(grid[axialCoord]);
        }
    }

    void Recursive(HexGridNode p_node, int p_iteration)
    {
        if (p_iteration == iteration)
            return;
        visited.Add(p_node);
        GenerateNeighbors(p_node);
        p_node.neighbors.ForEach(n =>
        {
            if (!visited.Contains(n))
                Recursive(n, p_iteration + 1);
        });
    }

    private void FixedUpdate()
    {
        //
    }

    public void ResetGrid()
    {
        //
    }
}
