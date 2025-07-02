using UnityEngine;

public class ReferencePrefabTest : MonoBehaviour
{
    public PrefabTest prefabTest;

    private void Awake()
    {
        transform.position = prefabTest.spriteRenderer.transform.position;
    }
}
