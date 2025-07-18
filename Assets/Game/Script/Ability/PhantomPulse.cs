using System.Collections;
using UnityEngine;

public class PhantomPulse : SkillBase
{
    Vector3 enemyDirection;
    CustomMono targetEnemy;

    public override void Awake()
    {
        base.Awake();
        cooldown = 2.5f;
        damage = defaultDamage = 25f;
        /* In this skill, this will be the number of animation we will play,
        we want to reuse as many fields as possible */
        maxAmmo = 9;
        /* In this skill, this will be the portion each variation hold in blend tree. */
        modifiedAngle = 1f / (maxAmmo - 1);
        successResult = new(true, true, cooldown);
        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new(
                ActionUse.GetCloser,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.4f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        base.WhileWaiting(p_direction);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            StartCoroutine(actionIE = TriggerIE(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerIE(Vector3 p_direction)
    {
        if (customMono.currentNearestEnemy == null)
        {
            customMono.SetUpdateDirectionIndicator(
                new(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
                UpdateDirectionIndicatorPriority.Low
            );
            SetBlend(GameManager.Instance.mainSkill2BlendHash, 0);
            /* fov: scale = vector3 */
            transform.position += new Vector3(
                Random.Range(-0.5f, .5f) * customMono.fieldOfView.transform.localScale.x,
                Random.Range(-0.5f, .5f) * customMono.fieldOfView.transform.localScale.y,
                0
            );
        }
        else
        {
            #region Skill variant
            /* In this skill, this will be the selected skill variation */
            targetEnemy = customMono.currentNearestEnemy;
            currentAmmo = Random.Range(1, maxAmmo);
            SetBlend(GameManager.Instance.mainSkill2BlendHash, modifiedAngle * currentAmmo);
            transform.position =
                targetEnemy.transform.position + new Vector3(Random.Range(-1, 1), 0, 0);

            while (!customMono.animationEventFunctionCaller.mainSkill2Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.animationEventFunctionCaller.mainSkill2Signal = false;
            switch (currentAmmo)
            {
                case 1: // fire down
                {
                    GameEffect t_shockwave = GameManager
                        .Instance.gameEffectPool.PickOne()
                        .gameEffect;
                    var t_shockwaveSO = GameManager.Instance.strongDudeShockwaveSO;
                    t_shockwave.Init(t_shockwaveSO);

                    var t_collideAndDamage = (CollideAndDamage)
                        t_shockwave.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
                    t_collideAndDamage.allyTags = customMono.allyTags;
                    t_collideAndDamage.collideDamage = damage * 0.5f;
                    t_collideAndDamage.pushDirection =
                        targetEnemy.rotationAndCenterObject.transform.position
                        - customMono.rotationAndCenterObject.transform.position;
                    t_shockwave.transform.parent = customMono.rotationAndCenterObject.transform;
                    t_shockwave.transform.localPosition = Vector3.zero;

                    break;
                }
                case 2: // fire punch
                {
                    enemyDirection =
                        targetEnemy.rotationAndCenterObject.transform.position
                        - customMono.rotationAndCenterObject.transform.position;
                    customMono.SetUpdateDirectionIndicator(
                        enemyDirection,
                        UpdateDirectionIndicatorPriority.Low
                    );
                    GameEffect t_knockUpCollider = GameManager
                        .Instance.gameEffectPool.PickOne()
                        .gameEffect;
                    var t_knockUpColliderSO = GameManager.Instance.knockUpColliderSO;
                    t_knockUpCollider.Init(t_knockUpColliderSO);

                    var t_collideAndDamage = (CollideAndDamage)
                        t_knockUpCollider.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
                    t_collideAndDamage.allyTags = customMono.allyTags;
                    t_collideAndDamage.collideDamage = damage * 0.5f;
                    t_knockUpCollider.transform.position = customMono.firePoint.transform.position;
                    t_knockUpCollider.rigidbody2D.AddForce(
                        enemyDirection.normalized * 5f,
                        ForceMode2D.Impulse
                    );
                    break;
                }
                case 3: // forward kick
                {
                    enemyDirection =
                        targetEnemy.rotationAndCenterObject.transform.position
                        - customMono.rotationAndCenterObject.transform.position;
                    customMono.SetUpdateDirectionIndicator(
                        enemyDirection,
                        UpdateDirectionIndicatorPriority.Low
                    );
                    GameEffect t_pushRandomCollider = GameManager
                        .Instance.gameEffectPool.PickOne()
                        .gameEffect;
                    var t_pushRandomColliderSO = GameManager.Instance.pushColliderSO;
                    t_pushRandomCollider.Init(t_pushRandomColliderSO);

                    var t_collideAndDamage = (CollideAndDamage)
                        t_pushRandomCollider.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
                    t_collideAndDamage.allyTags = customMono.allyTags;
                    t_collideAndDamage.collideDamage = damage * 0.5f;
                    t_collideAndDamage.pushDirection = enemyDirection;
                    t_pushRandomCollider.transform.position = customMono
                        .firePoint
                        .transform
                        .position;
                    t_pushRandomCollider.rigidbody2D.AddForce(
                        enemyDirection.normalized * 5f,
                        ForceMode2D.Impulse
                    );
                    break;
                }
                case 4: // Kame
                case 5: // left kick
                case 6: // punch
                {
                    StartCoroutine(FireDragon());
                    break;
                }
                case 7: // punch down
                {
                    enemyDirection =
                        targetEnemy.rotationAndCenterObject.transform.position
                        - customMono.rotationAndCenterObject.transform.position;
                    customMono.SetUpdateDirectionIndicator(
                        enemyDirection,
                        UpdateDirectionIndicatorPriority.Low
                    );
                    GameEffect t_pushRandomCollider = GameManager
                        .Instance.gameEffectPool.PickOne()
                        .gameEffect;
                    var t_pushColliderSO = GameManager.Instance.pushColliderSO;
                    t_pushRandomCollider.Init(t_pushColliderSO);

                    var t_collideAndDamage = (CollideAndDamage)
                        t_pushRandomCollider.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
                    t_collideAndDamage.allyTags = customMono.allyTags;
                    t_collideAndDamage.collideDamage = damage * 0.5f;
                    t_collideAndDamage.pushDirection = Vector2.down;
                    t_pushRandomCollider.transform.position = customMono
                        .firePoint
                        .transform
                        .position;
                    t_pushRandomCollider.rigidbody2D.AddForce(
                        enemyDirection.normalized * 5f,
                        ForceMode2D.Impulse
                    );
                    break;
                }
                case 8: // swipe right
                {
                    enemyDirection =
                        targetEnemy.rotationAndCenterObject.transform.position
                        - customMono.rotationAndCenterObject.transform.position;
                    customMono.SetUpdateDirectionIndicator(
                        enemyDirection,
                        UpdateDirectionIndicatorPriority.Low
                    );
                    GameEffect t_stunCollider = GameManager
                        .Instance.gameEffectPool.PickOne()
                        .gameEffect;
                    var t_stunColliderSO = GameManager.Instance.stunColliderSO;
                    t_stunCollider.Init(t_stunColliderSO);

                    var t_collideAndDamage = (CollideAndDamage)
                        t_stunCollider.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
                    t_collideAndDamage.allyTags = customMono.allyTags;
                    t_collideAndDamage.collideDamage = damage * 0.5f;
                    t_stunCollider.transform.position = customMono.firePoint.transform.position;
                    t_stunCollider.rigidbody2D.AddForce(
                        enemyDirection.normalized * 5f,
                        ForceMode2D.Impulse
                    );
                    break;
                }
                default:
                    break;
            }
            #endregion
            #region End Skill variant
            #endregion
        }

        while (!customMono.animationEventFunctionCaller.endMainSkill2)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill2 = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.currentAction = null;
    }

    IEnumerator FireDragon()
    {
        GameEffect t_phantomPulseDragon;
        GameEffectSO t_phantomPulseDragonSO;
        CollideAndDamage t_collideAndDamage;

        enemyDirection =
            targetEnemy.rotationAndCenterObject.transform.position
            - customMono.rotationAndCenterObject.transform.position;
        customMono.SetUpdateDirectionIndicator(
            enemyDirection,
            UpdateDirectionIndicatorPriority.Low
        );

        for (int i = 0; i < 2; i++)
        {
            t_phantomPulseDragon = GameManager.Instance.gameEffectPool.PickOne().gameEffect;
            t_phantomPulseDragonSO = GameManager.Instance.phantomPulseDragonSO;
            t_phantomPulseDragon.Init(t_phantomPulseDragonSO);

            t_collideAndDamage = (CollideAndDamage)
                t_phantomPulseDragon.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
            t_phantomPulseDragon.currentGameEffectSO.collideAndDamageSO.spawnedEffectOnCollide =
                customMono.meleeCollisionEffectSO;
            t_collideAndDamage.allyTags = customMono.allyTags;
            t_collideAndDamage.collideDamage = damage;
            t_phantomPulseDragon.transform.position = customMono.firePoint.transform.position;
            t_phantomPulseDragon.KeepFlyingAt(
                enemyDirection,
                t_phantomPulseDragonSO,
                EasingType.OutQuin
            );

            yield return new WaitForSeconds(0.2f);
        }
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_direction, p_duration));
    }

    IEnumerator BotTriggerIE(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill2 = false;
        customMono.currentAction = null;
    }
}
