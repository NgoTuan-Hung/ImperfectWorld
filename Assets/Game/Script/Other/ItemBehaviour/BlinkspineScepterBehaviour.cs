using System.Collections;
using UnityEngine;

public class BlinkspineScepterBehaviour : MonoBehaviour, IItemBehaviour
{
    public CustomMono CustomMono { get; set; }
    public Item Item { get; set; }
    float timer = 0;
    IEnumerator iEnumerator;

    public void OnAttach(CustomMono customMono, Item item)
    {
        CustomMono = customMono;
        Item = item;
        GameManager.Instance.battleStartCallback += TeleportOnBattle;
        GameManager.Instance.battleEndCallback += StopTeleport;
    }

    public void OnDetach()
    {
        GameManager.Instance.battleStartCallback -= TeleportOnBattle;
        GameManager.Instance.battleEndCallback -= StopTeleport;
    }

    void TeleportOnBattle()
    {
        timer = 0;
        StartCoroutine(iEnumerator = TeleportIE());
    }

    void StopTeleport()
    {
        StopCoroutine(iEnumerator);
    }

    IEnumerator TeleportIE()
    {
        while (true)
        {
            if (timer > 7.25f)
            {
                GameManager.Instance.vanishEffectPool.PickOneGameEffect().transform.position =
                    transform.position;
                transform.position = GameManager.Instance.GetRandomLocationOnMap();
                timer = 0;
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
