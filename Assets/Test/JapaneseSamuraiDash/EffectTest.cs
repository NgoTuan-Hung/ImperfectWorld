using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CollisionType
{
    None,
    Once,
    Multiple,
}

public class EffectTest : MonoBehaviour
{
    Animator animator;
    public CollisionType collisionType = CollisionType.None;
    public Action<Collision2D> onCollisionEnter2D = (collision2D) => { },
        onCollisionStay2D = (collision2D) => { },
        spawnEffectOnCollideEvt = (collision2D) => { };
    public bool spawnEffectOnCollide = false;
    public GameObject spawnEffectOnCollideObj;
    public bool knockUpOnCollide = false;
    public float travelDuration = 1f,
        travelSpeed = 16f;
    public bool isDeactivatedAfterTime = false;
    public float deactivateAfter = 0;
    Action onEnable = () => { };
    public AnimationCurve matPropOverTime;
    public bool changeMatPropOverTime;
    public string matPropName;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (isDeactivatedAfterTime)
            onEnable += () => StartCoroutine(DeactivateAfterTimeIE());

        if ((spriteRenderer = GetComponent<SpriteRenderer>()) == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (changeMatPropOverTime)
            onEnable += ChangeMatPropOverTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        if (spawnEffectOnCollide)
        {
            spawnEffectOnCollideEvt = (collision2D) =>
            {
                GameObject t_spawnEffectOnCollideObj = Instantiate(spawnEffectOnCollideObj);
                Vector3 t_contactPoint = collision2D
                    .GetContact(Random.Range(0, collision2D.contactCount))
                    .point;
                float t_randomBias = Random.Range(0, 1f);
                t_contactPoint =
                    collision2D.collider.bounds.center * t_randomBias
                    + (1 - t_randomBias) * t_contactPoint;

                t_spawnEffectOnCollideObj.transform.position = t_contactPoint;
            };
        }

        if (collisionType == CollisionType.Once) { }
        else if (collisionType == CollisionType.Multiple)
        {
            onCollisionStay2D = (collision2D) =>
            {
                spawnEffectOnCollideEvt(collision2D);
            };
        }

        if (knockUpOnCollide)
        {
            onCollisionEnter2D = (collision2D) =>
                collision2D.gameObject.GetComponent<Target>().HandleKnockUp();
        }
    }

    private void OnEnable()
    {
        onEnable();
    }

    IEnumerator DeactivateAfterTimeIE()
    {
        yield return new WaitForSeconds(deactivateAfter);
        gameObject.SetActive(false);
    }

    void ChangeMatPropOverTime()
    {
        StartCoroutine(ChangeMatPropOverTimeIE());
    }

    IEnumerator ChangeMatPropOverTimeIE()
    {
        float currentTime = 0;
        while (true)
        {
            spriteRenderer.material.SetFloat(
                matPropName,
                matPropOverTime.Evaluate(currentTime / deactivateAfter)
            );
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
    }

    public void Play()
    {
        animator.SetBool("Play", true);
    }

    public void End() => animator.SetBool("Play", false);

    // Update is called once per frame
    void Update() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        onCollisionEnter2D(collision);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        onCollisionStay2D(other);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Travel(Vector3 p_direction)
    {
        StartCoroutine(TravelIE(p_direction));
    }

    IEnumerator TravelIE(Vector3 p_direction)
    {
        float currentTime = 0;
        Vector3 rawDisplacement = travelSpeed * p_direction.normalized;
        while (currentTime < travelDuration)
        {
            transform.position += rawDisplacement * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
            currentTime += Time.deltaTime;
        }
    }
}
