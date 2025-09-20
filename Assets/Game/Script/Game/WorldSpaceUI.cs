using System.Collections;
using UnityEngine;

public class WorldSpaceUI : MonoSelfAware
{
    public Vector3 followOffset = new(-0.8f, 1.5f, 0);
    public float followSlowlyPositionLerpTime = 0.04f;

    public override void Awake()
    {
        base.Awake();
    }

    public void FollowSlowly(Transform master)
    {
        StartCoroutine(FollowSlowlyCoroutine(master));
    }

    IEnumerator FollowSlowlyCoroutine(Transform master)
    {
        Vector2 newVector2Position = master.position + followOffset,
            prevVector2Position,
            expectedVector2Position;
        float currentTime;

        while (true)
        {
            prevVector2Position = newVector2Position;
            /* Check current position */
            newVector2Position = master.position + followOffset;

            /* Start lerping position for specified duration if position change detected.*/
            if (prevVector2Position != newVector2Position)
            {
                currentTime = 0;
                while (currentTime < followSlowlyPositionLerpTime + Time.fixedDeltaTime)
                {
                    expectedVector2Position = Vector2.Lerp(
                        prevVector2Position,
                        newVector2Position,
                        currentTime / followSlowlyPositionLerpTime
                    );
                    transform.position = new Vector2(
                        expectedVector2Position.x,
                        expectedVector2Position.y
                    );

                    yield return new WaitForSeconds(currentTime += Time.fixedDeltaTime);
                }
            }

            transform.position = new Vector2(newVector2Position.x, newVector2Position.y);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void StartDamagePopup(Vector3 p_initialPos, float p_damage)
    {
        StartCoroutine(StartDamagePopupIE(p_initialPos, p_damage));
    }

    IEnumerator StartDamagePopupIE(Vector3 p_initialPos, float p_damage)
    {
        yield return GetComponent<TextPopupUI>().StartPopupIE(p_initialPos, p_damage);
        deactivate();
    }

    public void StartWeakenPopup(Vector3 p_initialPos)
    {
        StartCoroutine(StartWeakenPopupIE(p_initialPos));
    }

    IEnumerator StartWeakenPopupIE(Vector3 p_initialPos)
    {
        yield return GetComponent<TextPopupUI>().StartWeakenPopup(p_initialPos);
        deactivate();
    }
}
