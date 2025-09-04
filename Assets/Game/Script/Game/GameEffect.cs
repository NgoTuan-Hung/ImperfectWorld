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
    OutQuint,
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
    public TrailRenderer trailRenderer;
    Dictionary<EGameEffectBehaviour, IGameEffectBehaviour> behaviours = new();
    public GameEffectSO gameEffectSO;
    public Func<float, float> easingFunction;
    public List<BoxCollider2D> boxCollider2Ds = new();
    public List<CircleCollider2D> circleCollider2Ds = new();
    public List<PolygonCollider2D> polygonCollider2Ds = new();
    public Action onEnable = () => { };
    public new ParticleSystem particleSystem;

    public override void Awake()
    {
        base.Awake();
        animateObjects = GetComponentsInChildren<AnimateObject>().ToList();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playableDirector = GetComponent<PlayableDirector>();
        audioSource = GetComponent<AudioSource>();
        trailRenderer = GetComponent<TrailRenderer>();
        particleSystem = GetComponentInChildren<ParticleSystem>();

        GetAllColliders();
        GetAllBehaviours();
        Init();
    }

    private void OnEnable()
    {
        onEnable();
    }

    public void DeactivateAfterTime() => StartCoroutine(DeactivateAfterTimeIE());

    IEnumerator DeactivateAfterTimeIE()
    {
        yield return new WaitForSeconds(gameEffectSO.deactivateTime);
        deactivate();
    }

    void PlayAudioSource() => audioSource.Play();

    void RotateRandom() => transform.Rotate(0, 0, Random.Range(0, 360));

    void DoColorOverLifetime() => StartCoroutine(DoColorOverLifetimeIE());

    void PlayParticleSystem() => particleSystem.Play();

    void PlayTimeline() => playableDirector.Play();

    IEnumerator DoColorOverLifetimeIE()
    {
        float currentTime = 0;
        while (currentTime < gameEffectSO.deactivateTime)
        {
            animateObjects.ForEach(aO =>
            {
                aO.spriteRenderer.color = gameEffectSO.colorOverLifetimeGrad.Evaluate(
                    currentTime / gameEffectSO.deactivateTime
                );
            });
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
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
        var t_behaviours = GetComponents<IGameEffectBehaviour>();
        foreach (var t_behaviour in t_behaviours)
        {
            behaviours.Add(
                (EGameEffectBehaviour)
                    Enum.Parse(typeof(EGameEffectBehaviour), t_behaviour.GetType().Name),
                t_behaviour
            ); //
            t_behaviour.Initialize(this);
        }
    }

    public IGameEffectBehaviour GetBehaviour(EGameEffectBehaviour p_behaviour) =>
        behaviours[p_behaviour];

    public void Init()
    {
        if (gameEffectSO.isDeactivateAfterTime)
            onEnable += DeactivateAfterTime;
        if (gameEffectSO.playSound)
            onEnable += PlayAudioSource;
        if (gameEffectSO.randomRotation)
            onEnable += RotateRandom;
        if (gameEffectSO.isColoredOverLifetime)
            onEnable += DoColorOverLifetime;
        if (gameEffectSO.useParticleSystem)
            onEnable += PlayParticleSystem;
        if (gameEffectSO.isTimeline)
            onEnable += PlayTimeline;
    }

    public void KeepFlyingForward() => StartCoroutine(KeepFlyingAtCoroutine(transform.right));

    public void KeepFlyingAt(Vector3 direction)
    {
        StartCoroutine(KeepFlyingAtCoroutine(direction));
    }

    IEnumerator KeepFlyingAtCoroutine(Vector3 direction)
    {
        direction = direction.normalized * gameEffectSO.flyAtSpeed;
        while (true)
        {
            transform.position += direction;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void KeepFlyingAt(Vector3 p_direction, bool p_rotateToDirection, EasingType p_easingType)
    {
        if (p_rotateToDirection)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.WithZ(
                    Vector2.SignedAngle(Vector2.right, p_direction)
                )
            );
        }
        StartCoroutine(KeepFlyingAtCoroutine(p_direction, p_easingType));
    }

    IEnumerator KeepFlyingAtCoroutine(Vector3 p_direction, EasingType p_easingType)
    {
        switch (p_easingType)
        {
            case EasingType.OutQuint:
                easingFunction = EasingFunctions.OutQuint;
                break;
            default:
                yield break;
        }

        transform.localScale = transform.localScale.WithY(
            p_direction.x > 0 ? Math.Abs(transform.localScale.y) : -Math.Abs(transform.localScale.y)
        );
        p_direction = p_direction.normalized * gameEffectSO.flyAtSpeed;
        float currentTime = 0;
        while (true)
        {
            transform.position +=
                p_direction * (1 - easingFunction(currentTime / gameEffectSO.deactivateTime));
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
    }

    public void Follow(Transform master) => StartCoroutine(FollowCoroutine(master));

    IEnumerator FollowCoroutine(Transform master)
    {
        while (true)
        {
            transform.position = master.position + gameEffectSO.followOffset;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void PlaceAndLookAt(Vector3 p_position, Vector3 p_lookDir)
    {
        transform.SetPositionAndRotation(
            p_position,
            Quaternion.Euler(
                transform.rotation.eulerAngles.WithZ(Vector2.SignedAngle(Vector2.right, p_lookDir))
            )
        );
        animateObjects[0].transform.localScale = animateObjects[0]
            .transform.localScale.WithY(
                p_lookDir.x > 0
                    ? Math.Abs(animateObjects[0].transform.localScale.y)
                    : -Math.Abs(animateObjects[0].transform.localScale.y)
            );
    }

    public void SetUpCollideAndDamage(HashSet<string> p_allyTags, float p_damage)
    {
        var t_collideAndDamage = (CollideAndDamage)GetBehaviour(
            EGameEffectBehaviour.CollideAndDamage
        );
        t_collideAndDamage.allyTags = p_allyTags;
        t_collideAndDamage.collideDamage = p_damage;
    }

    static void DefaultDealtDamageEvent(float p_damage) { }

    public void PlaceAndLookAt(Vector3 p_position, Transform p_transform, float p_delay)
    {
        transform.position = p_position;
        StartCoroutine(LookAtIE(p_transform, p_delay));
    }

    IEnumerator LookAtIE(Transform p_transform, float p_delay)
    {
        Vector2 t_lookDir;
        while (true)
        {
            t_lookDir = p_transform.position - transform.position;

            yield return new WaitForSeconds(p_delay);

            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.WithZ(Vector2.SignedAngle(Vector2.right, t_lookDir))
            );
            transform.localScale = transform.localScale.WithY(
                t_lookDir.x > 0
                    ? Math.Abs(transform.localScale.y)
                    : -Math.Abs(transform.localScale.y)
            );
        }
    }

    public CollideAndDamage GetCollideAndDamage() =>
        GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;

    public void SetParentAndLocalPosAndRot(
        Transform p_parent,
        Vector3 p_localPos,
        Vector3 p_localRot
    )
    {
        transform.parent = p_parent;
        transform.localScale = Vector3.one;
        transform.SetLocalPositionAndRotation(p_localPos, Quaternion.Euler(p_localRot));
    }
}
