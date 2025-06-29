using UnityEngine;

public class TimelineSerializeSignal : MonoBehaviour
{
    public void OnSignal() => print(gameObject.name);
}
