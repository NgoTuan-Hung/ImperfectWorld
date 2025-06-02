using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class JSD : MonoBehaviour
{
    public AnimationClip attack2Clip;
    float clipLength;
    Animator animator;
    Vector3 defaultPos;
    public EffectTest effectTest, slashEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clipLength = attack2Clip.length;
        dashDefaultSpeed *= Time.deltaTime;
        animator = GetComponent<Animator>();
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
    
    public float dashSpeed, dashDefaultSpeed;
    public bool randomDir = false;
    Vector3 dashDir;
    IEnumerator StartDash()
    {
        defaultPos = transform.position;
        float currentTime = 0;
        animator.SetBool("Attack2", true);
        effectTest.transform.position = transform.position;
        effectTest.Play();
        
        if (randomDir) dashDir = new Vector3(Random.Range(0, 1f), Random.Range(-1f, 1f), 0).normalized;
        else dashDir = Vector3.right;
        
        while (currentTime < clipLength)
        {
            dashSpeed = dashDefaultSpeed - dashDefaultSpeed * EasingFunctions.OutQuint(currentTime / clipLength);
            transform.position += dashDir * dashSpeed;
        
            yield return new WaitForSeconds(Time.deltaTime);
            currentTime += Time.deltaTime;
        }
        animator.SetBool("Attack2", false);
        
        yield return new WaitForSeconds(0.1f);
        transform.position = defaultPos;
    }
    
    float slashAngle;
    public void SpawnSlash()
    {
        slashAngle = Vector2.SignedAngle(Vector2.right, dashDir);
        slashEffect.transform.parent.rotation = Quaternion.Euler(0, 0, slashAngle);
        slashEffect.Play();
    }
}
