using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullingMissile : MonoBehaviour, IGameEffectBehaviour
{
    public HashSet<string> allyTags = new();
    Action<Collider2D> onTriggerStay2D = other => { };
    public GameEffect GameEffect { get; set; }
    Dictionary<Collider2D, Vector3> localPos = new();

    private void Awake()
    {
        //
    }

    private void OnEnable()
    {
        localPos.Clear();
    }

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
        if (other.CompareTag("InvisibleWall"))
            GameEffect.deactivate();

        if (other.transform.parent != null)
        {
            /* Since parent will have customMono, not this */
            CustomMono t_customMono = GameManager.Instance.GetCustomMono(other);
            if (t_customMono != null)
            {
                if (!allyTags.Contains(t_customMono.tag))
                {
                    t_customMono.statusEffect.Stun(Time.fixedDeltaTime);
                    if (!localPos.ContainsKey(other))
                    {
                        localPos.Add(other, t_customMono.transform.position - transform.position);
                    }
                    else
                        t_customMono.transform.position = transform.position + localPos[other];
                }
                else { }
            }
        }
    }
}
