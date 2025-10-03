using UnityEngine;

public class RectSizeTest : MonoBehaviour
{
    private void Awake() { }

    private void Start()
    {
        print(((RectTransform)transform).rect.size);
    }
}
