using UnityEngine;

[DefaultExecutionOrder(-2)]
public class GridNodeObstacle : MonoBehaviour
{
    private void FixedUpdate()
    {
        GameManager.Instance.SetObstacle(transform.position);
    }
}
