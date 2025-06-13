using System;
using System.Collections.Generic;
using UnityEngine;

public class BlueHole : MonoBehaviour, IGameEffectBehaviour
{
    public HashSet<string> allyTags = new();
    public new Rigidbody2D rigidbody2D;
    float drawForce = 0.1f;
    public GameEffect GameEffect { get; set; }

    public void Initialize(GameEffect gameEffect)
    {
        GameEffect = gameEffect;
    }

    private void OnEnable()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) { }

    private void OnTriggerStay2D(Collider2D other)
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
