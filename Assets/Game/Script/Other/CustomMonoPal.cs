using UnityEngine;

/// <summary>
/// Any class which has a connection with CustomMono.
/// </summary>
public class CustomMonoPal : MonoBehaviour
{
    public CustomMono customMono;

    public virtual void Awake()
    {
        customMono = GetComponent<CustomMono>();
    }
}
