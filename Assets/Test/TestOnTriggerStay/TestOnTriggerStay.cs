using UnityEngine;

public class TestOnTriggerStay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerStay2D(Collider2D other)
    {
        print("OnTriggerStay2D");
    }
}
