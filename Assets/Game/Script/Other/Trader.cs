using System.Collections;
using UnityEngine;

public class Trader : MonoBehaviour
{
    Animator animator;
    int traderIntHash = Animator.StringToHash("Trader");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(ChangePose());
    }

    IEnumerator ChangePose()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            animator.SetInteger(traderIntHash, Random.Range(1, 5));
        }
    }
}
