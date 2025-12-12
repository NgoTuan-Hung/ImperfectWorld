using UnityEngine;
using UnityEngine.EventSystems;

public class TestUGUIEventCascade : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        print("OnDrag");
    }
}
