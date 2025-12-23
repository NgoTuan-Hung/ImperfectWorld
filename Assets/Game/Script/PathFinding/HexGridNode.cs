using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public enum HexGridNodeType
{
    Normal,
    Obstacle,
}

public class HexGridNode : FastPriorityQueueNode
{
    public HexGridNodeType type = HexGridNodeType.Normal;
    public AxialCoord axialCoord;
    public Vector2 pos;
    public List<HexGridNode> neighbors = new();
    public HexGridNode cameFrom = null;
    public float costSoFar = 0;
    public bool visited = false;
    public bool inQueue = false;
    public SpriteRenderer visual;

    public HexGridNode(AxialCoord axialCoord)
    {
        this.axialCoord = axialCoord;
        pos =
            0.5f
            * new Vector2(
                1.5f * axialCoord.Q,
                Mathf.Sqrt(3f) / 2f * axialCoord.Q + Mathf.Sqrt(3f) * axialCoord.R
            );
    }

    public void Reset() => type = HexGridNodeType.Normal;

    public void ResetForPathFinding()
    {
        visited = false;
        inQueue = false;
    }

    public void ShowVisual() => visual.gameObject.SetActive(true);

    public void HideVisual() => visual.gameObject.SetActive(false);

    public void SetVisual(GameObject visual)
    {
        this.visual = visual.GetComponent<SpriteRenderer>();
        visual.transform.position = pos;
    }

    public void SetAsPermanentObstacle()
    {
        type = HexGridNodeType.Obstacle;
        visual.color = Color.black;
        HexGridManager.Instance.RemoveFromResetGrid(this);
    }

    public void SwitchToDefaultVisual()
    {
        visual.color = HexGridManager.defaultNodeColor;
        visual.sharedMaterial = HexGridManager.defaultNodeMaterial;
    }

    public void SwitchToHighlightedVisual()
    {
        visual.color = HexGridManager.highlightedNodeColor;
        visual.sharedMaterial = HexGridManager.highlightedNodeMaterial;
    }

    public void SwitchToOccupiedVisual()
    {
        visual.color = HexGridManager.occupiedColor;
        visual.sharedMaterial = HexGridManager.occupiedNodeMaterial;
    }
}
