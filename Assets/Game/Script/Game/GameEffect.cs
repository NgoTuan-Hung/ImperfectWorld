using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kryz.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public enum EasingType
{
    OutQuin,
}

public enum EGameEffectBehaviour
{
    None,
    CollideAndDamage,
    BlueHole,
}

public class GameEffect : MonoSelfAware
{
    public new Rigidbody2D rigidbody2D;
    Animator animator;
    PlayableDirector playableDirector;
    AudioSource audioSource;
    public SpriteRenderer spriteRenderer,
        secondarySpriteRenderer;
    public List<GameObject> trackReferences;
    public BoxCollider2D boxCollider2D;
    public PolygonCollider2D polygonCollider2D;
    public CircleCollider2D circleCollider2D;
    public TrailRenderer trailRenderer;
    Dictionary<EGameEffectBehaviour, IGameEffectBehaviour> behaviours = new();
    public GameEffectSO currentGameEffectSO;
    public Func<float, float> easingFunction;

    public override void Awake()
    {
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();
        playableDirector = GetComponent<PlayableDirector>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = spriteRenderer.gameObject.GetComponent<Animator>();
        secondarySpriteRenderer = spriteRenderer
            .transform.Find("SecondarySpriteRenderer")
            .gameObject.GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();

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

                t_behaviour.Initialize(this);
            }
        }
    }

    public IGameEffectBehaviour GetBehaviour(EGameEffectBehaviour p_behaviour) =>
        behaviours[p_behaviour];

    public GameEffect Init(GameEffectSO p_gameEffectSO)
    {
        currentGameEffectSO = p_gameEffectSO;
        /* Turn off behaviors, rigidbody, colliders and reset to default state */
        ResetGameEffect();

        /* Set up game effect */
        HandleGameEffect(p_gameEffectSO);

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

        return this;
    }

    void ResetGameEffect()
    {
        playableDirector.time = 0;
        playableDirector.Evaluate();
        playableDirector.Stop();

        foreach (var behaviour in behaviours.Values)
        {
            behaviour.Disable();
        }

        transform.parent = null;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        boxCollider2D.enabled = false;
        polygonCollider2D.enabled = false;
        circleCollider2D.enabled = false;
        secondarySpriteRenderer.sprite = null;
        trailRenderer.enabled = false;
    }

    void HandleGameEffect(GameEffectSO p_gameEffectSO)
    {
        spriteRenderer.transform.SetLocalPositionAndRotation(
            p_gameEffectSO.gameEffectPrefab.spriteRenderer.transform.localPosition,
            p_gameEffectSO.gameEffectPrefab.spriteRenderer.transform.localRotation
        );
        spriteRenderer.transform.localScale = p_gameEffectSO
            .gameEffectPrefab
            .spriteRenderer
            .transform
            .localScale;
        spriteRenderer.color = p_gameEffectSO.gameEffectPrefab.spriteRenderer.color;
        spriteRenderer.spriteSortPoint = p_gameEffectSO
            .gameEffectPrefab
            .spriteRenderer
            .spriteSortPoint;
        spriteRenderer.material = p_gameEffectSO.gameEffectPrefab.spriteRenderer.sharedMaterial;
        spriteRenderer.sortingLayerName = p_gameEffectSO
            .gameEffectPrefab
            .spriteRenderer
            .sortingLayerName;
        spriteRenderer.sortingOrder = p_gameEffectSO.gameEffectPrefab.spriteRenderer.sortingOrder;

        if (p_gameEffectSO.useSecondarySpriteRenderer)
        {
            secondarySpriteRenderer.gameObject.transform.SetLocalPositionAndRotation(
                p_gameEffectSO.gameEffectPrefab.secondarySpriteRenderer.transform.localPosition,
                p_gameEffectSO.gameEffectPrefab.secondarySpriteRenderer.transform.localRotation
            );
            secondarySpriteRenderer.gameObject.transform.localScale = p_gameEffectSO
                .gameEffectPrefab
                .secondarySpriteRenderer
                .transform
                .localScale;
            secondarySpriteRenderer.sprite = p_gameEffectSO
                .gameEffectPrefab
                .secondarySpriteRenderer
                .sprite;
            secondarySpriteRenderer.color = p_gameEffectSO
                .gameEffectPrefab
                .secondarySpriteRenderer
                .color;
            secondarySpriteRenderer.material = p_gameEffectSO
                .gameEffectPrefab
                .secondarySpriteRenderer
                .sharedMaterial;
            secondarySpriteRenderer.sortingLayerName = p_gameEffectSO
                .gameEffectPrefab
                .secondarySpriteRenderer
                .sortingLayerName;
            secondarySpriteRenderer.sortingOrder = p_gameEffectSO
                .gameEffectPrefab
                .secondarySpriteRenderer
                .sortingOrder;
        }

        animator.runtimeAnimatorController = p_gameEffectSO.runtimeAnimatorController;

        if (p_gameEffectSO.isDeactivateAfterTime)
            StartCoroutine(DeactivateAfterTimeIE(p_gameEffectSO.deactivateTime));

        audioSource.resource = p_gameEffectSO.audioSource.resource;
        audioSource.pitch = p_gameEffectSO.audioSource.pitch;
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

        if (p_gameEffectSO.useCircleCollider)
        {
            circleCollider2D.enabled = true;
            circleCollider2D.offset = p_gameEffectSO.circleCollider2D.offset;
            circleCollider2D.radius = p_gameEffectSO.circleCollider2D.radius;
        }

        if (p_gameEffectSO.useTrailRenderer)
        {
            trailRenderer.enabled = true;
            trailRenderer.widthMultiplier = p_gameEffectSO
                .gameEffectPrefab
                .trailRenderer
                .widthMultiplier;
            trailRenderer.widthCurve = p_gameEffectSO.gameEffectPrefab.trailRenderer.widthCurve;
            trailRenderer.time = p_gameEffectSO.gameEffectPrefab.trailRenderer.time;
            trailRenderer.colorGradient = p_gameEffectSO
                .gameEffectPrefab
                .trailRenderer
                .colorGradient;
            trailRenderer.material = p_gameEffectSO.gameEffectPrefab.trailRenderer.sharedMaterial;
            trailRenderer.sortingLayerName = p_gameEffectSO
                .gameEffectPrefab
                .trailRenderer
                .sortingLayerName;
            trailRenderer.sortingOrder = p_gameEffectSO.gameEffectPrefab.trailRenderer.sortingOrder;
        }

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

    public void KeepFlyingAt(
        Vector3 p_direction,
        GameEffectSO p_gameEffectSO,
        EasingType p_easingType
    )
    {
        StartCoroutine(KeepFlyingAtCoroutine(p_direction, p_gameEffectSO, p_easingType));
    }

    IEnumerator KeepFlyingAtCoroutine(
        Vector3 p_direction,
        GameEffectSO p_gameEffectSO,
        EasingType p_easingType
    )
    {
        switch (p_easingType)
        {
            case EasingType.OutQuin:
                easingFunction = EasingFunctions.OutQuint;
                break;
            default:
                yield break;
        }

        spriteRenderer.transform.localScale = spriteRenderer.transform.localScale.WithX(
            spriteRenderer.transform.localScale.x > 0
                ? spriteRenderer.transform.localScale.x
                : -spriteRenderer.transform.localScale.x
        );
        transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(Vector2.right, p_direction)
        );
        p_direction = p_direction.normalized * p_gameEffectSO.flyAtSpeed;
        float currentTime = 0;
        while (true)
        {
            transform.position +=
                p_direction * (1 - easingFunction(currentTime / p_gameEffectSO.deactivateTime));
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
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
