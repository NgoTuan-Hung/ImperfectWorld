using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;
    public Action interact = () => { };

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        GameManager.Instance.AddNPC(this);
    }

    private void Start()
    {
        //
    }
}
