using UnityEngine;

public class TestDomainReloading : MonoBehaviour
{
    public static int value = 0;

    private void Awake()
    {
        print($"Awake {value}");
    }

    [ContextMenu("Increase")]
    void Increase()
    {
        value += 1;
        // Dang yeah sure
    }
}
