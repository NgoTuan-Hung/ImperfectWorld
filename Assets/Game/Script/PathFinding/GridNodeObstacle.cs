using UnityEngine;

[DefaultExecutionOrder(-2)]
public class GridNodeObstacle : CustomMonoPal
{
    public override void Start()
    {
        base.Start();
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        GameManager.Instance.SetObstacle(customMono.boxCollider2D);
    }
}
