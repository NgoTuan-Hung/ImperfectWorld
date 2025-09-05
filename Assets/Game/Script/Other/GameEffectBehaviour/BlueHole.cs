using System;
using System.Collections.Generic;
using UnityEngine;

public class BlueHole : MonoBehaviour, IGameEffectBehaviour
{
    public HashSet<string> allyTags = new();
    float drawForce = 0.1f;
    Action<Collider2D> onTriggerStay2D = (Collider2D other) => { };

    public GameEffect GameEffect { get; set; }

    public void Initialize(GameEffect gameEffect)
    {
        GameEffect = gameEffect;
        onTriggerStay2D = OnTriggerStay2DLogic;
    }

    private void OnTriggerEnter2D(Collider2D other) { }

    private void OnTriggerStay2D(Collider2D other)
    {
        onTriggerStay2D(other);
    }

    private void OnTriggerStay2DLogic(Collider2D other)
    {
        if (other.transform.parent != null)
        {
            /* Since parent will have customMono, not this */
            CustomMono t_customMono = GameManager.Instance.GetCustomMono(
                other.transform.parent.gameObject
            );
            if (t_customMono != null)
            {
                if (!allyTags.Contains(t_customMono.tag))
                {
                    Vector2 drawDirection = transform.position - t_customMono.transform.position;
                    t_customMono.rigidbody2D.AddForce(
                        drawForce
                            / Math.Clamp(drawDirection.magnitude, 1f, float.MaxValue)
                            * drawDirection,
                        ForceMode2D.Impulse
                    );
                }
                else { }
            }
        }
    }
}
