using UnityEngine;
using UnityEngine.EventSystems;

public class TestRaycast : MonoBehaviour, IDragHandler
{
    public enum TestRaycastMode
    {
        WorldSpace,
        ScreenSpace,
    }

    public TestRaycastMode testRaycastMode = TestRaycastMode.ScreenSpace;

    public void OnDrag(PointerEventData eventData)
    {
        if (testRaycastMode == TestRaycastMode.ScreenSpace)
            transform.position = Camera.main.ScreenToWorldPoint(eventData.position.WithZ(100));
        else
            transform.position = transform.position = Camera.main.ScreenToWorldPoint(
                eventData.position.WithZ(10)
            );
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
