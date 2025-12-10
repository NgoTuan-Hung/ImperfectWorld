using UnityEngine;
using UnityEngine.EventSystems;

public class TestRayCastOnMouseDrag : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        print("OnDrag");
    }
}
