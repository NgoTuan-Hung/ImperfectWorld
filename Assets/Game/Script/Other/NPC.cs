using System;
using UnityEngine;

public class NPC : MonoSelfAware
{
    public BoxCollider2D boxCollider2D;
    public Action interact = () => { };

    public override void Awake()
    {
        base.Awake();
        boxCollider2D = GetComponent<BoxCollider2D>();
        GameManager.Instance.AddNPC(this);
    }

    private void Start()
    {
        //
    }
}
