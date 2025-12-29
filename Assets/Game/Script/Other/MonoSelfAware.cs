using System;
using UnityEngine;

public class MonoSelfAware : MonoBehaviour
{
    public Action deactivate;

    public virtual void Awake()
    {
        deactivate = Deactivate;
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
