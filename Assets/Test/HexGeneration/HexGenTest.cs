using UnityEngine;
using UnityEngine.InputSystem;

public class HexGenTest : MonoBehaviour
{
    HexGridManager hexGridManager;

    private void Awake()
    {
        hexGridManager = GetComponent<HexGridManager>();
    }

    private void FixedUpdate()
    {
        if (Pointer.current.press.isPressed)
        {
            hexGridManager
                .GetHexSpriteDebug(
                    Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue().WithZ(10))
                )
                .color = Color.red;
        }
    }
}
