using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualizeAlgorithm : MonoBehaviour
{
    public static VisualizeAlgorithm Instance;
    public Vector4 bound = new Vector4(-1, 1, 1, -1);
    public GameObject gridSquare,
        worldSpaceCanvas;
    public GridSquare startPoint,
        destPoint;
    public List<GridSquare> gridSquares = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
        var width = bound.z - bound.x;
        var height = bound.y - bound.w;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var square = Instantiate(gridSquare);
                square.transform.SetParent(worldSpaceCanvas.transform, false);
                square.transform.position = new Vector3(bound.x + i, bound.y - j, 0);
                gridSquares.Add(
                    square
                        .GetComponent<GridSquare>()
                        .Init(
                            new(
                                square.transform.position.x + 0.5f,
                                square.transform.position.y - 0.5f
                            )
                        )
                );
            }
        }
    }

    // Update is called once per frame
    void Update() { }

    public void AssignStartPoint(GridSquare square)
    {
        startPoint = square;
    }

    public void AssignDestPoint(GridSquare square)
    {
        destPoint = square;
        if (startPoint != null)
        {
            gridSquares.ForEach(gQ =>
            {
                if (gQ.state == GridSquareState.Normal)
                {
                    var originCost = Vector2.Distance(startPoint.pos, gQ.pos);
                    var destCost = Vector2.Distance(destPoint.pos, gQ.pos);
                    gQ.SetValues(originCost + destCost, originCost, destCost);
                    gQ.isCalculated = true;
                }
            });

            OpenAvailable(startPoint);
        }
    }

    readonly float sqrt2 = Mathf.Sqrt(2);

    public void OpenAvailable(GridSquare square)
    {
        gridSquares
            .Where(gS => Vector2.Distance(gS.pos, square.pos) <= sqrt2 && gS != square)
            .ToList()
            .ForEach(gS =>
            {
                gS.ChangeToAvailable();
            });
    }
}
