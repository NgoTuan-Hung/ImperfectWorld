using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance;
    public SpriteRenderer bound;
    GameObject hexNodeParent;
    public GameObject hexNodePrefab;
    Dictionary<AxialCoord, HexGridNode> grid = new();
    FastPriorityQueue<HexGridNode> queue;
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

    List<HexGridNode> open = new(),
        next = new();

    float q1,
        r1,
        x1,
        z1,
        y1,
        dx1,
        dy1,
        dz1;
    int rx1,
        ry1,
        rz1;
    Action resetGrid = () => { },
        resetGridForPathFinding = () => { },
        showVisual = () => { },
        hideVisual = () => { };
    public static HexGridNode border = new(new AxialCoord(int.MaxValue, int.MaxValue));
    public static Color defaultNodeColor = Color.green.WithAlpha(0.5f);
    public static Color highlightedNodeColor = Color.white,
        occupiedColor = Color.white;
    public static Material defaultNodeMaterial,
        highlightedNodeMaterial,
        occupiedNodeMaterial;
    Dictionary<HexGridNode, CustomMono> occupiers = new();
    Dictionary<CustomMono, HexGridNode> nodeOccupy = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        defaultNodeMaterial = Resources.Load("Material/HexNode") as Material;
        highlightedNodeMaterial = Resources.Load("Material/HexNodeHighlighted") as Material;
        occupiedNodeMaterial = Resources.Load("Material/HexNodeOccupied") as Material;
        hexNodeParent = GameObject.Find("HexGrid");
        border.type = HexGridNodeType.Obstacle;
        HexGridNode origin = SetupNewNode(new AxialCoord(0, 0));
        open.Add(origin);
        visited.Add(origin);

        Generate();
    }

    public List<HexGridNode> SolvePath(HexGridNode start, HexGridNode end)
    {
        var path = new List<HexGridNode>();
        float priority;
        resetGridForPathFinding();
        queue = new(grid.Count);
        queue.Enqueue(start, 0);
        start.cameFrom = null;
        start.costSoFar = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            current.visited = true;
            if (current == end)
            {
                path.Add(current);
                while (current.cameFrom != null)
                {
                    current = current.cameFrom;
                    path.Add(current);
                }
                return path;
            }

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor.visited || neighbor.type == HexGridNodeType.Obstacle)
                {
                    continue;
                }

                var newCost = current.costSoFar + Vector2.Distance(current.pos, neighbor.pos);

                if (!neighbor.inQueue)
                {
                    neighbor.costSoFar = newCost;
                    priority = newCost + Vector2.Distance(neighbor.pos, end.pos);
                    queue.Enqueue(neighbor, priority);
                    neighbor.inQueue = true;
                    neighbor.cameFrom = current;
                }
                else if (newCost < neighbor.costSoFar)
                {
                    neighbor.costSoFar = newCost;
                    priority = newCost + Vector2.Distance(neighbor.pos, end.pos);
                    queue.UpdatePriority(neighbor, priority);
                    neighbor.cameFrom = current;
                }
            }
        }

        return path;
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
                SetupNewNode(axialCoord);
            parent.neighbors.Add(grid[axialCoord]);
        }
    }

    HexGridNode SetupNewNode(AxialCoord axialCoord)
    {
        var hGN = new HexGridNode(axialCoord);
        grid.Add(axialCoord, hGN);
        resetGrid += hGN.Reset;
        resetGridForPathFinding += hGN.ResetForPathFinding;
        hGN.SetVisual(Instantiate(hexNodePrefab, hexNodeParent.transform));
        showVisual += hGN.ShowVisual;
        hideVisual += hGN.HideVisual;
        return hGN;
    }

    void Generate()
    {
        while (open.Count > 0)
        {
            open.ForEach(hGN =>
            {
                GenerateNeighbors(hGN);
            });

            open.ForEach(hGN =>
            {
                hGN.neighbors.ForEach(nB =>
                {
                    if (!visited.Contains(nB))
                    {
                        visited.Add(nB);
                        if (CheckPosInsideBound(nB.pos))
                            next.Add(nB);
                        else
                            nB.SetAsPermanentObstacle();
                    }
                });
            });

            open = next;
            next = new();
        }

        // open.ForEach(hGN => hGN.type = HexGridNodeType.Obstacle);
    }

    public HexGridNode GetNodeAtPosition(Vector2 pos)
    {
        // Step 1: Convert to fractional axial coordinates
        q1 = (2f / 3f * pos.x) / 0.5f;
        r1 = (-1f / 3f * pos.x + Mathf.Sqrt(3f) / 3f * pos.y) / 0.5f;

        // Step 2: Convert to cube coordinates
        x1 = q1;
        z1 = r1;
        y1 = -x1 - z1;

        // Step 3: Round cube coordinates
        rx1 = Mathf.RoundToInt(x1);
        ry1 = Mathf.RoundToInt(y1);
        rz1 = Mathf.RoundToInt(z1);

        dx1 = Mathf.Abs(rx1 - x1);
        dy1 = Mathf.Abs(ry1 - y1);
        dz1 = Mathf.Abs(rz1 - z1);

        if (dx1 > dy1 && dx1 > dz1)
            rx1 = -ry1 - rz1;
        else if (dy1 > dz1)
            ry1 = -rx1 - rz1;
        else
            rz1 = -rx1 - ry1;

        // Step 4: Convert back to axial
        return grid.GetValueOrDefault(new(rx1, rz1)) ?? border;
    }

    public void MarkNodeAsObstacle(Vector2 pos) =>
        GetNodeAtPosition(pos).type = HexGridNodeType.Obstacle;

    public void MarkNodeAsNormal(Vector2 pos) =>
        GetNodeAtPosition(pos).type = HexGridNodeType.Normal;

    private void FixedUpdate()
    {
        ResetGrid();
    }

    public void ResetGrid()
    {
        resetGrid();
    }

    bool CheckPosInsideBound(Vector2 pos)
    {
        return pos.x >= bound.bounds.min.x
            && pos.x <= bound.bounds.max.x
            && pos.y >= bound.bounds.min.y
            && pos.y <= bound.bounds.max.y;
    }

    public void ShowVisual() => showVisual();

    public void HideVisual() => hideVisual();

    public void RemoveFromResetGrid(HexGridNode node)
    {
        resetGrid -= node.Reset;
    }

    public void SetOccupiedNode(CustomMono customMono, Vector2 position)
    {
        var node = GetNodeAtPosition(position);
        SetOccupiedNode(customMono, node);
    }

    public void SetOccupiedNode(CustomMono customMono, HexGridNode node)
    {
        occupiers[node] = customMono;
        nodeOccupy[customMono] = node;
        customMono.transform.position = node.pos;
        node.SwitchToOccupiedVisual();
    }

    public void SetOccupyNextAvailable(CustomMono customMono)
    {
        foreach (var node in grid.Values)
        {
            if (node.type != HexGridNodeType.Obstacle && !IsOccupied(node))
            {
                SetOccupiedNode(customMono, node);
                return;
            }
        }
    }

    public void SetOccupiedNodeHighlight(CustomMono customMono, HexGridNode node)
    {
        occupiers[node] = customMono;
        nodeOccupy[customMono] = node;
        node.SwitchToHighlightedVisual();
    }

    public HexGridNode GetOccupyNode(CustomMono customMono) => nodeOccupy[customMono];

    public CustomMono GetOccupier(HexGridNode node) => occupiers[node];

    public void RemoveOccupy(CustomMono customMono)
    {
        var node = nodeOccupy[customMono];
        if (node != null)
        {
            occupiers.Remove(node);
            nodeOccupy.Remove(customMono);
            node.SwitchToDefaultVisual();
        }
    }

    public void RemoveOccupy(HexGridNode node)
    {
        if (node != null)
        {
            var customMono = occupiers[node];
            occupiers.Remove(node);
            nodeOccupy.Remove(customMono);
            node.SwitchToDefaultVisual();
        }
    }

    public void ClearAllOccupy()
    {
        foreach (var node in occupiers.Keys)
        {
            node.SwitchToDefaultVisual();
        }

        occupiers.Clear();
        nodeOccupy.Clear();
    }

    public bool IsOccupied(HexGridNode node)
    {
        if (occupiers.ContainsKey(node))
            return true;
        return false;
    }
}
