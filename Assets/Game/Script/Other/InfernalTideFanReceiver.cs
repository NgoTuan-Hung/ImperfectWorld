using System;
using System.Collections.Generic;
using UnityEngine;

public class InfernalTideFanReceiver : MonoBehaviour, IGameEffectBehaviour
{
    Action<Collider2D> onTriggerEnter2D = (Collider2D other) => { };

    public GameEffect GameEffect { get; set; }

    public void Disable()
    {
        onTriggerEnter2D = DisabledOnTriggerEnter2D;
    }

    public void Enable(GameEffectSO p_gameEffectSO)
    {
        onTriggerEnter2D = OnTriggerEnter2DLogic;
    }

    public void Initialize(GameEffect gameEffect)
    {
        GameEffect = gameEffect;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter2D(other);
    }

    private void DisabledOnTriggerEnter2D(Collider2D other) { }

    GameObject collideWithGO;

    private void OnTriggerEnter2DLogic(Collider2D other)
    {
        if (GameManager.Instance.colliderOwner.TryGetValue(other.GetHashCode(), out collideWithGO))
        {
            if (collideWithGO.tag.Equals("InfernalTideFan"))
            {
                print("Yay");
                onTriggerEnter2D = DisabledOnTriggerEnter2D;
            }
        }
    }
}
