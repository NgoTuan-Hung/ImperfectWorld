using UnityEngine;
using UnityEngine.EventSystems;

public class TestUGUIEventCascadeChild : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        print("OnDrag Child");
    }
}
