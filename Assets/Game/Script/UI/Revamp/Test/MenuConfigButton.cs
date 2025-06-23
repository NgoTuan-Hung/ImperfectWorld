using UnityEngine;
using UnityEngine.EventSystems;

public class MenuConfigButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        print(eventData.position);
    }
}
