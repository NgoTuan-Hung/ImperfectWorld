using UnityEngine;

public class UIRManager : MonoBehaviour
{
    public static Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
}
