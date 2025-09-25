using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum GridSquareState
{
    Normal,
    Available,
    Marked,
    Start,
    Dest,
    Obstacle,
}

public class GridSquare : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
{
    public bool isCalculated = false;
    public Vector2 pos;
    public GridSquareState state = GridSquareState.Normal;
    public Image image,
        obstacleIMG;
    public TextMeshProUGUI totalCostTMP,
        originCostTMP,
        destCostTMP;
    public static Color transparentWhite = new(1, 1, 1, 0);
    public List<GridSquare> neighbors = new();

    public GridSquare Init(Vector2 pos)
    {
        this.pos = pos;
        image = transform.Find("Image").GetComponent<Image>();
        obstacleIMG = transform.Find("ObstacleIMG").GetComponent<Image>();
        totalCostTMP = transform.Find("TotalCostTMP").GetComponent<TextMeshProUGUI>();
        originCostTMP = transform.Find("OriginCostTMP").GetComponent<TextMeshProUGUI>();
        destCostTMP = transform.Find("DestCostTMP").GetComponent<TextMeshProUGUI>();
        return this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCalculated)
        {
            state = GridSquareState.Marked;
            image.color = Color.red;
            VisualizeAlgorithm.Instance.OpenAvailable(this);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (Keyboard.current.nKey.isPressed)
        {
            obstacleIMG.color = Color.white;
            state = GridSquareState.Start;
            VisualizeAlgorithm.Instance.AssignStartPoint(this);
        }

        if (Keyboard.current.dKey.isPressed)
        {
            obstacleIMG.color = Color.red;
            state = GridSquareState.Dest;
            VisualizeAlgorithm.Instance.AssignDestPoint(this);
        }

        if (Keyboard.current.bKey.isPressed)
        {
            state = GridSquareState.Obstacle;
            obstacleIMG.color = Color.black;
        }
    }

    public void SetValues(float totalCost, float originCost, float destCost)
    {
        totalCostTMP.text = totalCost.ToString("F1");
        originCostTMP.text = originCost.ToString("F1");
        destCostTMP.text = destCost.ToString("F1");
    }

    public void ChangeToAvailable()
    {
        if (state != GridSquareState.Marked)
        {
            image.color = Color.green;
            state = GridSquareState.Available;
        }
    }

    public void AddNeighbor(GridSquare neighbor)
    {
        neighbors.Add(neighbor);
    }
}
