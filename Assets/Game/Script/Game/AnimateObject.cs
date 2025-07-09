using UnityEngine;

public class AnimateObject : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
