using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    public ItemUI owner;
    public static Vector3 offset = new Vector3(0.3f, 0.25f, 0);

    public void Init(ItemUI owner)
    {
        this.owner = owner;
    }

    void FixedUpdate()
    {
        transform.position = owner.transform.position + offset;
    }
}
