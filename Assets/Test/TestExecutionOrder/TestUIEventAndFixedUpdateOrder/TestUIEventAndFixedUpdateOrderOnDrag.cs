using UnityEngine;
using UnityEngine.EventSystems;

public class TestUIEventAndFixedUpdateOrderOnDrag : MonoBehaviour, IDragHandler
{
    public TestUIEventAndFixedUpdateOrderManager manager;

    private void Awake()
    {
        manager = FindAnyObjectByType<TestUIEventAndFixedUpdateOrderManager>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        print(manager.test);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
