using System.Collections;
using UnityEngine;

public class MercuryGraspBehaviour : MonoBehaviour, IItemBehaviour
{
    public CustomMono CustomMono { get; set; }
    public Item Item { get; set; }
    IEnumerator iEnumerator;
    FloatStatModifier reflexModifier = new(0, ModifierType.Additive);
    Vector2 previousPos;

    public void OnAttach(CustomMono customMono, Item item)
    {
        CustomMono = customMono;
        Item = item;
        CustomMono.stat.reflex.AddModifier(reflexModifier);
        GameManager.Instance.battleStartCallback += StartModifyingReflex;
        GameManager.Instance.battleEndCallback += StopModifyingReflex;
    }

    public void OnDetach()
    {
        CustomMono.stat.reflex.RemoveModifier(reflexModifier);
        GameManager.Instance.battleStartCallback -= StartModifyingReflex;
        GameManager.Instance.battleEndCallback -= StopModifyingReflex;
    }

    void StartModifyingReflex()
    {
        previousPos = transform.position;
        reflexModifier.value = 0;
        StartCoroutine(iEnumerator = StartModifyingReflexIE());
    }

    void StopModifyingReflex()
    {
        StopCoroutine(iEnumerator);
    }

    IEnumerator StartModifyingReflexIE()
    {
        while (true)
        {
            reflexModifier.value += Vector2.Distance(transform.position, previousPos);
            CustomMono.stat.reflex.RecalculateFinalValue();
            previousPos = transform.position;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
