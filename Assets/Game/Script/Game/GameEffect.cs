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
    InfernalTideFanReceiver,
}

public class GameEffect : MonoSelfAware
{
    public new Rigidbody2D rigidbody2D;
    PlayableDirector playableDirector;
    AudioSource audioSource;
    public List<AnimateObject> animateObjects;
    public List<GameObject> trackReferences;
    public TrailRenderer trailRenderer;
    Dictionary<EGameEffectBehaviour, IGameEffectBehaviour> behaviours = new();
    public GameEffectSO currentGameEffectSO;
    public Func<float, float> easingFunction;
    public List<BoxCollider2D> boxCollider2Ds = new();
    public List<CircleCollider2D> circleCollider2Ds = new();
    public List<PolygonCollider2D> polygonCollider2Ds = new();

    public override void Awake()
    {
        base.Awake();

        animateObjects = GetComponentsInChildren<AnimateObject>().ToList();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playableDirector = GetComponent<PlayableDirector>();
        audioSource = GetComponent<AudioSource>();
        trailRenderer = GetComponent<TrailRenderer>();

        GetAllColliders();
        GetAllBehaviours();
    }

    void GetAllColliders()
    {
        boxCollider2Ds = GetComponentsInChildren<BoxCollider2D>().ToList();
        circleCollider2Ds = GetComponentsInChildren<CircleCollider2D>().ToList();
        polygonCollider2Ds = GetComponentsInChildren<PolygonCollider2D>().ToList();

        boxCollider2Ds.ForEach(bC =>
            GameManager.Instance.colliderOwner.Add(bC.GetHashCode(), gameObject)
        );
        circleCollider2Ds.ForEach(cC =>
            GameManager.Instance.colliderOwner.Add(cC.GetHashCode(), gameObject)
        );
        polygonCollider2Ds.ForEach(pC =>
            GameManager.Instance.colliderOwner.Add(pC.GetHashCode(), gameObject)
        );
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
        foreach (var behaviour in p_gameEffectSO.gameEffectBehaviours)
        {
            switch (behaviour)
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
                case EGameEffectBehaviour.InfernalTideFanReceiver:
                {
                    behaviours[EGameEffectBehaviour.InfernalTideFanReceiver].Enable(p_gameEffectSO);
                    break;
                }
                default:
                    break;
            }
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

        animateObjects.ForEach(aO =>
        {
            aO.animator.runtimeAnimatorController = null;
            aO.spriteRenderer.sprite = null;
            aO.gameObject.SetActive(false);
        });
        trailRenderer.enabled = false;
    }

    void HandleGameEffect(GameEffectSO p_gameEffectSO)
    {
        gameObject.tag = p_gameEffectSO.tag;
        rigidbody2D.excludeLayers = p_gameEffectSO.collisionExcludeLayerMask;

        for (int i = 0; i < p_gameEffectSO.gameEffectPrefab.animateObjects.Count; i++)
        {
            animateObjects[i].gameObject.SetActive(true);
            animateObjects[i]
                .spriteRenderer.transform.SetLocalPositionAndRotation(
                    p_gameEffectSO
                        .gameEffectPrefab
                        .animateObjects[i]
                        .spriteRenderer
                        .transform
                        .localPosition,
                    p_gameEffectSO
                        .gameEffectPrefab
                        .animateObjects[i]
                        .spriteRenderer
                        .transform
                        .localRotation
                );
            animateObjects[i].spriteRenderer.transform.localScale = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .spriteRenderer
                .transform
                .localScale;
            animateObjects[i].spriteRenderer.color = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .spriteRenderer
                .color;
            animateObjects[i].spriteRenderer.spriteSortPoint = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .spriteRenderer
                .spriteSortPoint;
            animateObjects[i].spriteRenderer.material = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .spriteRenderer
                .sharedMaterial;
            animateObjects[i].spriteRenderer.sortingLayerName = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .spriteRenderer
                .sortingLayerName;
            animateObjects[i].spriteRenderer.sortingOrder = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .spriteRenderer
                .sortingOrder;

            animateObjects[i].animator.runtimeAnimatorController = p_gameEffectSO
                .gameEffectPrefab
                .animateObjects[i]
                .animator
                .runtimeAnimatorController;
        }

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

        #region Colliders Handler
        for (int i = 0; i < p_gameEffectSO.gameEffectPrefab.boxCollider2Ds.Count; i++)
        {
            boxCollider2Ds[i].gameObject.SetActive(true);
            boxCollider2Ds[i].offset = p_gameEffectSO.gameEffectPrefab.boxCollider2Ds[i].offset;
            boxCollider2Ds[i].size = p_gameEffectSO.gameEffectPrefab.boxCollider2Ds[i].size;
        }
        for (
            int i = p_gameEffectSO.gameEffectPrefab.boxCollider2Ds.Count;
            i < boxCollider2Ds.Count;
            i++
        )
            boxCollider2Ds[i].gameObject.SetActive(false);

        for (int i = 0; i < p_gameEffectSO.gameEffectPrefab.circleCollider2Ds.Count; i++)
        {
            circleCollider2Ds[i].gameObject.SetActive(true);
            circleCollider2Ds[i].offset = p_gameEffectSO
                .gameEffectPrefab
                .circleCollider2Ds[i]
                .offset;
            circleCollider2Ds[i].radius = p_gameEffectSO
                .gameEffectPrefab
                .circleCollider2Ds[i]
                .radius;
        }
        for (
            int i = p_gameEffectSO.gameEffectPrefab.circleCollider2Ds.Count;
            i < circleCollider2Ds.Count;
            i++
        )
            circleCollider2Ds[i].gameObject.SetActive(false);

        for (int i = 0; i < p_gameEffectSO.gameEffectPrefab.polygonCollider2Ds.Count; i++)
        {
            polygonCollider2Ds[i].gameObject.SetActive(true);
            polygonCollider2Ds[i].offset = p_gameEffectSO
                .gameEffectPrefab
                .polygonCollider2Ds[i]
                .offset;
            polygonCollider2Ds[i].points = p_gameEffectSO
                .gameEffectPrefab
                .polygonCollider2Ds[i]
                .points;
        }
        for (
            int i = p_gameEffectSO.gameEffectPrefab.polygonCollider2Ds.Count;
            i < polygonCollider2Ds.Count;
            i++
        )
            polygonCollider2Ds[i].gameObject.SetActive(false);

        #endregion

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
            animateObjects.ForEach(aO =>
            {
                if (aO.gameObject.activeSelf)
                    aO.spriteRenderer.color = p_gradient.Evaluate(currentTime / p_lifeTime);
            });
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

        transform.localScale = transform.localScale.WithY(
            p_direction.x > 0 ? transform.localScale.y : -transform.localScale.y
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
