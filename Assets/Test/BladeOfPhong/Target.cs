using System.Collections;
using System.Linq;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool isKnockingUp = false;
    public float knockUpInitialVelocity = 20f,
        knockUpObjVelocity,
        acceleration = -10f;
    GameObject fallingIndicator;

    private void Awake()
    {
        fallingIndicator = transform
            .GetComponentsInChildren<Transform>(true)
            .First(x => x.name == "FallingIndicator")
            .gameObject;
    }

    public void HandleKnockUp()
    {
        if (isKnockingUp)
            knockUpObjVelocity += knockUpInitialVelocity;
        else
            StartCoroutine(KnockUpIE(transform.GetChild(0).gameObject));
    }

    IEnumerator KnockUpIE(GameObject p_knockUpObj)
    {
        fallingIndicator.gameObject.SetActive(true);
        isKnockingUp = true;
        float currentY = p_knockUpObj.transform.position.y;
        knockUpObjVelocity = knockUpInitialVelocity;
        while (p_knockUpObj.transform.position.y >= currentY)
        {
            p_knockUpObj.transform.position += Vector3.up * knockUpObjVelocity;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            knockUpObjVelocity += acceleration * Time.fixedDeltaTime;
        }

        p_knockUpObj.transform.position = p_knockUpObj.transform.position.WithY(currentY);
        isKnockingUp = false;
        fallingIndicator.gameObject.SetActive(false);
    }
}
