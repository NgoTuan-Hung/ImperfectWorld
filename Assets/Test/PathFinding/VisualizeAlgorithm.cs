// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Priority_Queue;
// using UnityEngine;

// public class VisualizeAlgorithm : MonoBehaviour
// {
//     public static VisualizeAlgorithm Instance;
//     public Vector4 bound = new Vector4(-1, 1, 1, -1);
//     public GameObject gridSquare,
//         worldSpaceCanvas;
//     public GridSquare startPoint,
//         destPoint;
//     public List<List<GridSquare>> gridSquares = new();
//     public GridSquare border;
//     SimplePriorityQueue<GridSquare> queue = new();
//     Dictionary<GridSquare, GridSquare> cameFrom = new();
//     Dictionary<GridSquare, float> costSoFar = new();

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         Instance = this;
//         worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
//         var width = bound.z - bound.x;
//         var height = bound.y - bound.w;
//         SetupBorder();

//         for (int i = 0; i < width; i++)
//         {
//             gridSquares.Add(new List<GridSquare>());
//             for (int j = 0; j < height; j++)
//             {
//                 var square = Instantiate(gridSquare);
//                 square.transform.SetParent(worldSpaceCanvas.transform, false);
//                 square.transform.position = new Vector3(bound.x + i, bound.y - j, 0);
//                 gridSquares[i]
//                     .Add(
//                         square
//                             .GetComponent<GridSquare>()
//                             .Init(
//                                 new(
//                                     square.transform.position.x + 0.5f,
//                                     square.transform.position.y - 0.5f
//                                 )
//                             )
//                     );
//             }
//         }

//         for (int i = 0; i < gridSquares.Count; i++)
//         {
//             for (int j = 0; j < gridSquares[i].Count; j++)
//             {
//                 for (int x = i - 1; x < i + 2; x++)
//                 {
//                     for (int y = j - 1; y < j + 2; y++)
//                     {
//                         if (x >= 0 && x < width && y >= 0 && y < height)
//                         {
//                             if (x != i || y != j)
//                             {
//                                 gridSquares[i][j].neighbors.Add(gridSquares[x][y]);
//                             }
//                         }
//                         else
//                             gridSquares[i][j].neighbors.Add(border);
//                     }
//                 }
//             }
//         }
//     }

//     private void SetupBorder()
//     {
//         border = GetComponent<GridSquare>();
//         border.state = GridSquareState.Obstacle;
//     }

//     // Update is called once per frame
//     void Update() { }

//     public void AssignStartPoint(GridSquare square)
//     {
//         startPoint?.RemoveStart();
//         startPoint = square;
//         PathFinding();
//     }

//     public void AssignDestPoint(GridSquare square)
//     {
//         destPoint?.RemoveDestination();
//         destPoint = square;
//         PathFinding();
//     }

//     public void AssignObstacle(GridSquare square)
//     {
//         PathFinding();
//     }

//     void DebugPath(GridSquare current)
//     {
//         if (current != startPoint)
//         {
//             DebugPath(cameFrom[current]);
//             current.ChangeToPath();
//         }
//     }

//     void RemoveAllPath()
//     {
//         foreach (var list in gridSquares)
//         {
//             foreach (var square in list)
//             {
//                 if (square.state == GridSquareState.Path)
//                     square.RemovePath();
//             }
//         }
//     }

//     public void PathFinding()
//     {
//         if (startPoint != null && destPoint != null)
//         {
//             RemoveAllPath();
//             queue = new();
//             cameFrom = new();
//             costSoFar = new();
//             queue.Enqueue(startPoint, 0);
//             cameFrom.Add(startPoint, null);
//             costSoFar.Add(startPoint, 0);

//             while (queue.Count > 0)
//             {
//                 var current = queue.Dequeue();
//                 if (current == destPoint)
//                 {
//                     DebugPath(cameFrom[current]);
//                     break;
//                 }

//                 foreach (var neighbor in current.neighbors)
//                 {
//                     if (neighbor.state == GridSquareState.Obstacle)
//                     {
//                         continue;
//                     }

//                     var newCost = costSoFar[current] + Vector2.Distance(current.pos, neighbor.pos);
//                     if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
//                     {
//                         costSoFar[neighbor] = newCost;
//                         var priority = newCost + Vector2.Distance(neighbor.pos, destPoint.pos);
//                         if (!queue.Contains(neighbor))
//                             queue.Enqueue(neighbor, priority);
//                         else
//                             queue.UpdatePriority(neighbor, priority);
//                         cameFrom[neighbor] = current;
//                     }
//                 }
//             }
//         }
//     }
// }
