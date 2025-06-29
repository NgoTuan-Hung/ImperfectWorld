using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class GameEffect : MonoSelfAware
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    private bool isDeactivatedAfterTime = false;
    Action deactiveAfterTime = () => { };

    [SerializeField]
    private float deactivateTime = 1f;
    PlayableDirector playableDirector;
    public bool isTimeline = false;
    public bool randomRotation = false;
    public float flyAtSpeed = 0.03f;
    Action onEnable = () => { };
    public Vector3 followOffset = new(-0.8f, 1.5f, 0);
    public float followSlowlyPositionLerpTime = 0.04f;
    AudioSource audioSource;
    public bool playSoundOnEnable = false;
    public Material material;
    public Vector3 effectLocalPosition,
        effectLocalRotation;
    public bool isColoredOverLifetime = false;
    public Gradient colorOverLifetime;
    Dictionary<Type, IGameEffectBehaviour> behaviours = new();

    public override void Awake()
    {
        base.Awake();

        playableDirector = GetComponent<PlayableDirector>();
        audioSource = GetComponent<AudioSource>();

        /* Don't put these on Start because Start run after OnEnable, if so the first time they
        show up, they won't handle deactivation correctly */
        if (isDeactivatedAfterTime)
        {
            deactiveAfterTime += () =>
            {
                StartCoroutine(DeactivateAfterTimeCoroutine(deactivateTime));
            };
        }
        if (isTimeline)
            onEnable += () => playableDirector.Play();
        if (playSoundOnEnable)
            onEnable += () => audioSource.Play();
        if (randomRotation)
            onEnable += () => transform.Rotate(0, 0, Random.Range(0, 360));
        if (isColoredOverLifetime)
            onEnable += DoColorOverLifetime;

        GetAllBehaviours();
    }

    void GetAllBehaviours()
    {
        var t_behaviours = GetComponents<IGameEffectBehaviour>();
        foreach (var behaviour in t_behaviours)
        {
            behaviours.Add(behaviour.GetType(), behaviour);
            behaviour.Initialize(this);
        }
    }

    public T GetBehaviour<T>()
        where T : class, IGameEffectBehaviour
    {
        if (behaviours.TryGetValue(typeof(T), out var behaviour))
            return behaviour as T;
        return null;
    }

    private void OnEnable()
    {
        deactiveAfterTime();
        onEnable();
    }

    public void DeactivateAfterTime(float deactivateTime) =>
        StartCoroutine(DeactivateAfterTimeCoroutine(deactivateTime));

    IEnumerator DeactivateAfterTimeCoroutine(float deactivateTime)
    {
        yield return new WaitForSeconds(deactivateTime);
        deactivate();
    }

    void DoColorOverLifetime() => StartCoroutine(ColorOverLifetimeIE());

    IEnumerator ColorOverLifetimeIE()
    {
        float currentTime = 0;
        while (currentTime < deactivateTime)
        {
            spriteRenderer.color = colorOverLifetime.Evaluate(currentTime / deactivateTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
    }

    public void KeepFlyingAt(Vector3 direction)
    {
        StartCoroutine(KeepFlyingAtCoroutine(direction));
    }

    IEnumerator KeepFlyingAtCoroutine(Vector3 direction)
    {
        direction = direction.normalized * flyAtSpeed;
        while (true)
        {
            transform.position += direction;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
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

    public void Follow(Transform master) => StartCoroutine(FollowCoroutine(master));

    IEnumerator FollowCoroutine(Transform master)
    {
        while (true)
        {
            transform.position = master.position + followOffset;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
