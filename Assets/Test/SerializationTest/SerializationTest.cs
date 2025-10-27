using UnityEngine;

public class SerializationTest : MonoBehaviour
{
    public SerializationTest stat;
    public float value;
    public float finalValue;

    [ContextMenu("Test")]
    public void Test()
    {
        stat.finalValue = value;
    }
}
