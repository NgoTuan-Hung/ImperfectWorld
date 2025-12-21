using UnityEngine;
using UnityEngine.EventSystems;

public class TestImageSize : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        print(gameObject.name);
    }
}
