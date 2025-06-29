using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public enum EGameEffectBehaviour
{
    None,
    CollideAndDamage,
    BlueHole,
}

public class GameEffectRevamp : MonoSelfAware
{
    public new Rigidbody2D rigidbody2D;
    Animator animator;
    PlayableDirector playableDirector;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    public List<GameObject> trackReferences;
    public BoxCollider2D boxCollider2D;
    public PolygonCollider2D polygonCollider2D;
    Dictionary<EGameEffectBehaviour, IGameEffectBehaviour> behaviours = new();

    public override void Awake()
    {
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playableDirector = GetComponent<PlayableDirector>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GetAllBehaviours();
    }

    void GetAllBehaviours()
    {
        foreach (EGameEffectBehaviour behaviour in Enum.GetValues(typeof(EGameEffectBehaviour)))
        {
            if (behaviour != EGameEffectBehaviour.None)
            {
                var t_behaviour =
                    GetComponent(Type.GetType(behaviour.ToString())) as IGameEffectBehaviour;
                behaviours.Add(behaviour, t_behaviour);

                // t_behaviour.Initialize(this);
            }
        }
    }

    public IGameEffectBehaviour GetBehaviour(EGameEffectBehaviour p_behaviour) =>
        behaviours[p_behaviour];

    public void Init(GameEffectSO p_gameEffectSO)
    {
        /* Turn off behaviors, rigidbody, colliders and reset to default state */
        Reset();

        animator.runtimeAnimatorController = p_gameEffectSO.animator.runtimeAnimatorController;

        if (p_gameEffectSO.isDeactivateAfterTime)
            StartCoroutine(DeactivateAfterTimeIE(p_gameEffectSO.deactivateTime));

        if (p_gameEffectSO.isTimeline)
        {
            playableDirector.playableAsset = p_gameEffectSO.timelineAsset;

            /* Handle timeline rebind */
            var tracks = p_gameEffectSO.timelineAsset.GetOutputTracks().ToArray();

            for (int i = 0; i < tracks.Count(); i++)
            {
                playableDirector.SetGenericBinding(tracks[i], trackReferences[i]);
            }

            playableDirector.Play();
        }

        if (p_gameEffectSO.playSound)
            PlayAudioSource();

        if (p_gameEffectSO.randomRotation)
            transform.Rotate(0, 0, Random.Range(0, 360));

        if (p_gameEffectSO.isColoredOverLifetime)
            DoColorOverLifetime(
                p_gameEffectSO.colorOverLifetimeGrad,
                p_gameEffectSO.deactivateTime
            );

        /* Handle colliders */
        if (p_gameEffectSO.useBoxCollider)
        {
            boxCollider2D.enabled = true;
            boxCollider2D.offset = p_gameEffectSO.boxCollider2D.offset;
            boxCollider2D.size = p_gameEffectSO.boxCollider2D.size;
        }

        if (p_gameEffectSO.usePolygonCollider)
        {
            polygonCollider2D.enabled = true;
            polygonCollider2D.offset = p_gameEffectSO.polygonCollider2D.offset;
            polygonCollider2D.points = p_gameEffectSO.polygonCollider2D.points;
        }

        /* Turn on behavior */
        switch (p_gameEffectSO.useBehaviour)
        {
            case EGameEffectBehaviour.CollideAndDamage:
            {
                behaviours[EGameEffectBehaviour.CollideAndDamage].Enable(p_gameEffectSO);
                break;
            }
            case EGameEffectBehaviour.BlueHole:
            {
                behaviours[EGameEffectBehaviour.BlueHole].Enable(p_gameEffectSO);
                break;
            }
            default:
                break;
        }
    }

    void Reset()
    {
        foreach (var behaviour in behaviours.Values)
        {
            behaviour.Disable();
        }

        boxCollider2D.enabled = false;
        polygonCollider2D.enabled = false;
    }

    IEnumerator DeactivateAfterTimeIE(float deactivateTime)
    {
        yield return new WaitForSeconds(deactivateTime);
        deactivate();
    }

    public void PlayAudioSource() => audioSource.Play();

    void DoColorOverLifetime(Gradient p_gradient, float p_lifeTime) =>
        StartCoroutine(ColorOverLifetimeIE(p_gradient, p_lifeTime));

    IEnumerator ColorOverLifetimeIE(Gradient p_gradient, float p_lifeTime)
    {
        float currentTime = 0;
        while (currentTime < p_lifeTime)
        {
            spriteRenderer.color = p_gradient.Evaluate(currentTime / p_lifeTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
    }

    public void KeepFlyingAt(Vector3 direction, GameEffectSO p_gameEffectSO)
    {
        StartCoroutine(KeepFlyingAtCoroutine(direction, p_gameEffectSO));
    }

    IEnumerator KeepFlyingAtCoroutine(Vector3 direction, GameEffectSO p_gameEffectSO)
    {
        direction = direction.normalized * p_gameEffectSO.flyAtSpeed;
        while (true)
        {
            transform.position += direction;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void FollowSlowly(Transform master, GameEffectSO p_gameEffectSO)
    {
        StartCoroutine(FollowSlowlyCoroutine(master, p_gameEffectSO));
    }

    IEnumerator FollowSlowlyCoroutine(Transform master, GameEffectSO p_gameEffectSO)
    {
        Vector2 newVector2Position = master.position + p_gameEffectSO.followOffset,
            prevVector2Position,
            expectedVector2Position;
        float currentTime;

        while (true)
        {
            prevVector2Position = newVector2Position;
            /* Check current position */
            newVector2Position = master.position + p_gameEffectSO.followOffset;

            /* Start lerping position for specified duration if position change detected.*/
            if (prevVector2Position != newVector2Position)
            {
                currentTime = 0;
                while (
                    currentTime < p_gameEffectSO.followSlowlyPositionLerpTime + Time.fixedDeltaTime
                )
                {
                    expectedVector2Position = Vector2.Lerp(
                        prevVector2Position,
                        newVector2Position,
                        currentTime / p_gameEffectSO.followSlowlyPositionLerpTime
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

    public void Follow(Transform master, GameEffectSO p_gameEffectSO) =>
        StartCoroutine(FollowCoroutine(master, p_gameEffectSO));

    IEnumerator FollowCoroutine(Transform master, GameEffectSO p_gameEffectSO)
    {
        while (true)
        {
            transform.position = master.position + p_gameEffectSO.followOffset;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
