using System.Collections;
using UnityEngine;

public class FlashLogic : ActionLogic
{
    public FlashLogic(BaseAction baseAction)
        : base(baseAction) { }

    public IEnumerator Flash(Vector2 p_dir, float p_dist, float p_duration)
    {
        baseAction.customMono.boxCollider2D.enabled = false;
        baseAction.customMono.combatCollider2D.enabled = false;
        baseAction.customMono.spriteRenderer.enabled = false;
        baseAction.transform.AddPos(p_dir.normalized * p_dist);

        yield return new WaitForSeconds(p_duration);

        baseAction.customMono.boxCollider2D.enabled = true;
        baseAction.customMono.combatCollider2D.enabled = true;
        baseAction.customMono.spriteRenderer.enabled = true;
    }
}
