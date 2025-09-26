using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathFindingFinalTest : MonoBehaviour
{
    GridManager gridManager;
    GridNode start,
        end;
    public GameObject gridNodePrefab;
    GameObject startNode,
        endNode;
    public float cooldown = 0.2f;
    private float lastTime = 0f;
    public GameObject mouse;
    Dictionary<GridNode, GameObject> obstacles = new();
    public GameObject agent;
    public float agentMoveSpeed = 1f,
        safeStopDistance = 0.1f;
    List<GameObject> previousPath = new();
    public BoxCollider2D collider2D;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        startNode = CreateNode(new Vector3(10, 10), Color.red);
        endNode = CreateNode(new Vector3(10, -10), Color.yellow);
        agent.transform.localScale = new Vector3(gridManager.nodeSize, gridManager.nodeSize, 1);
    }

    private void Update()
    {
        mouse.transform.position = Camera.main.ScreenToWorldPoint(
            Pointer.current.position.ReadValue().WithZ(10)
        );

        if (Time.time - lastTime > cooldown)
        {
            lastTime = Time.time;

            if (Keyboard.current.sKey.isPressed)
            {
                start = gridManager.GetNodeAtPosition(
                    Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue().WithZ(10))
                );
                startNode.transform.position = start.pos;
            }

            if (Keyboard.current.eKey.isPressed)
            {
                end = gridManager.GetNodeAtPosition(
                    Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue().WithZ(10))
                );
                endNode.transform.position = end.pos;
            }

            if (Keyboard.current.bKey.isPressed)
            {
                var gN = gridManager.GetNodeAtPosition(
                    Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue().WithZ(10))
                );
                if (!obstacles.ContainsKey(gN))
                {
                    obstacles.Add(gN, CreateNode(gN.pos, Color.black));
                    gN.type = GridNodeType.Obstacle;
                }
            }

            if (Keyboard.current.xKey.isPressed)
            {
                var gN = gridManager.GetNodeAtPosition(
                    Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue().WithZ(10))
                );
                if (obstacles.ContainsKey(gN))
                {
                    gN.Reset();
                    Destroy(obstacles[gN]);
                    obstacles.Remove(gN);
                }
            }

            if (Keyboard.current.fKey.isPressed && start != null && end != null)
            {
                foreach (var node in previousPath)
                {
                    Destroy(node);
                }
                previousPath.Clear();

                var path = gridManager.SolvePath(start, end);
                for (int i = 1; i < path.Count - 1; i++)
                {
                    previousPath.Add(CreateNode(path[i].pos, Color.green));
                }
                StopAllCoroutines();
                StartCoroutine(AgentMoveIE(path));
            }
        }
    }

    void ChangeSpriteColor(GameObject obj, Color color)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    GameObject CreateNode(Vector3 pos, Color color = default)
    {
        if (color == default)
            color = Color.white;
        GameObject obj = Instantiate(gridNodePrefab, pos, Quaternion.identity);
        obj.transform.localScale = new Vector3(gridManager.nodeSize, gridManager.nodeSize, 1);
        ChangeSpriteColor(obj, color);
        return obj;
    }

    IEnumerator AgentMoveIE(List<GridNode> path)
    {
        safeStopDistance = agentMoveSpeed / 2f + 0.01f;
        agent.transform.position = path[^1].pos;
        for (int i = path.Count - 2; i > 0; i--)
        {
            while (Vector2.Distance(agent.transform.position, path[i].pos) > safeStopDistance)
            {
                AgentMove(path[i].pos.AsVector3() - agent.transform.position);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            agent.transform.position = path[i].pos;
        }
    }

    void AgentMove(Vector2 vector2) =>
        agent.transform.position += (agentMoveSpeed * vector2.normalized).AsVector3();
}
