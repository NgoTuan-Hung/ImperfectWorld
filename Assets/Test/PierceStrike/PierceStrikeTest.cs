using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class PierceStrikeTest : MonoBehaviour
{
    public AnimationClip attack3Clip;
    float clipLength;
    Animator animator;
    Vector3 defaultPos;
    public EffectTest effectTest,
        slashEffect;
    GameObject rotationObject;
    EffectTest pierceStrikeEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clipLength = attack3Clip.length;
        dashDefaultSpeed *= Time.deltaTime;
        animator = GetComponent<Animator>();
        rotationObject = transform.Find("RotationObject").gameObject;
        pierceStrikeEffect = GetComponentInChildren<EffectTest>(true);
    }

    public bool start = false;

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(StartDash());
        }
    }

    public float dashSpeed,
        dashDefaultSpeed;
    public bool randomDir = false;
    Vector3 dashDir;

    IEnumerator StartDash()
    {
        defaultPos = transform.position;
        float currentTime = 0;
        effectTest.transform.position = transform.position;
        effectTest.gameObject.SetActive(true);

        animator.SetBool("Attack3", true);

        if (randomDir)
            dashDir = new Vector3(Random.Range(0, 1f), Random.Range(-1f, 1f), 0).normalized;
        else
            dashDir = Vector3.right;

        while (currentTime < clipLength)
        {
            dashSpeed =
                dashDefaultSpeed
                - dashDefaultSpeed * EasingFunctions.OutQuint(currentTime / clipLength);
            transform.position += dashDir * dashSpeed;

            yield return new WaitForSeconds(Time.deltaTime);
            currentTime += Time.deltaTime;
        }
        animator.SetBool("Attack3", false);

        yield return new WaitForSeconds(0.1f);
        transform.position = defaultPos;
    }

    public void SpawnPierceEffect()
    {
        rotationObject.transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector3.SignedAngle(dashDir, Vector3.right, -Vector3.forward)
        );
        pierceStrikeEffect.gameObject.SetActive(true);
    }
}
