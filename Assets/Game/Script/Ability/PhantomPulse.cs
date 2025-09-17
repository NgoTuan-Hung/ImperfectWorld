// using System.Collections;
// using UnityEngine;

// public class PhantomPulse : SkillBase
// {
//     public override void Awake()
//     {
//         base.Awake();
//     }

//     public override void OnEnable()
//     {
//         base.OnEnable();
//     }

//     public override void AddActionManuals()
//     {
//         base.AddActionManuals();
//         botActionManuals.Add(
//             new(
//                 ActionUse.GetCloser,
//                 (p_doActionParamInfo) =>
//                     BotTrigger(
//                         p_doActionParamInfo.centerToTargetCenterDirection,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.4f)
//             )
//         );
//     }

//     public override void Start()
//     {
//         base.Start();
//         StatChangeRegister();
//     }

//     public override void Config()
//     {
//         GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 2.5f;
//         /* In this skill, this will be the number of animation we will play,
//         we want to reuse as many fields as possible */
//         GetActionField<ActionIntField>(ActionFieldName.Variants).value = 9;
//         /* In this skill, this will be the portion each variation hold in blend tree. */
//         GetActionField<ActionFloatField>(ActionFieldName.Blend).value =
//             1f / (GetActionField<ActionIntField>(ActionFieldName.Variants).value - 1);
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 10f;
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         /* Also use damage, target, selectedVariant, gameeffect, direction */
//     }

//     public override void StatChangeRegister()
//     {
//         base.StatChangeRegister();
//         customMono.stat.might.finalValueChangeEvent += RecalculateStat;
//     }

//     public override void RecalculateStat()
//     {
//         base.RecalculateStat();
//         GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
//             customMono.stat.might.FinalValue * 1.5f;
//     }

//     public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
//     {
//         base.WhileWaiting(p_direction);
//     }

//     public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
//     {
//         if (
//             customMono.stat.currentManaPoint.Value
//             < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
//         )
//             return failResult;
//         else if (canUse && !customMono.actionBlocking)
//         {
//             canUse = false;
//             customMono.actionBlocking = true;
//             customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
//             ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
//                     direction
//                 )
//             );
//             StartCoroutine(CooldownCoroutine());
//             customMono.currentAction = this;
//             customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
//                 ActionFieldName.ManaCost
//             ).value;

//             return successResult;
//         }

//         return failResult;
//     }

//     IEnumerator TriggerIE(Vector3 p_direction)
//     {
//         if (customMono.botSensor.currentNearestEnemy == null)
//         {
//             customMono.SetUpdateDirectionIndicator(
//                 new(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
//                 UpdateDirectionIndicatorPriority.Low
//             );
//             SetBlend(GameManager.Instance.mainSkill2BlendHash, 0);
//             /* fov: scale = vector3 */
//             transform.position += new Vector3(
//                 Random.Range(-0.5f, .5f) * customMono.fieldOfView.transform.localScale.x,
//                 Random.Range(-0.5f, .5f) * customMono.fieldOfView.transform.localScale.y,
//                 0
//             );
//         }
//         else
//         {
//             #region Skill variant
//             GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value = customMono
//                 .botSensor
//                 .currentNearestEnemy;
//             GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value = Random.Range(
//                 1,
//                 GetActionField<ActionIntField>(ActionFieldName.Variants).value
//             );
//             SetBlend(
//                 GameManager.Instance.mainSkill2BlendHash,
//                 GetActionField<ActionFloatField>(ActionFieldName.Blend).value
//                     * GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value
//             );
//             transform.position =
//                 GetActionField<ActionCustomMonoField>(
//                     ActionFieldName.Target
//                 ).value.transform.position + new Vector3(Random.Range(-1, 1), 0, 0);

//             while (
//                 !customMono.animationEventFunctionCaller.GetSignalVals(
//                     EAnimationSignal.MainSkill2Signal
//                 )
//             )
//                 yield return new WaitForSeconds(Time.fixedDeltaTime);

//             customMono.animationEventFunctionCaller.SetSignal(
//                 EAnimationSignal.MainSkill2Signal,
//                 false
//             );
//             switch (GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value)
//             {
//                 case 1: // fire down
//                 {
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
//                         GameManager.Instance.strongDudeShockwavePool.PickOne().gameEffect;

//                     var t_collideAndDamage = (CollideAndDamage)
//                         GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                             .value.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
//                     t_collideAndDamage.allyTags = customMono.allyTags;
//                     t_collideAndDamage.collideDamage =
//                         GetActionField<ActionFloatField>(ActionFieldName.Damage).value * 0.5f;
//                     t_collideAndDamage.pushEnemyOnCollideForce = 20;
//                     t_collideAndDamage.pushDirection =
//                         GetActionField<ActionCustomMonoField>(
//                             ActionFieldName.Target
//                         ).value.rotationAndCenterObject.transform.position
//                         - customMono.rotationAndCenterObject.transform.position;
//                     GetActionField<ActionGameEffectField>(
//                         ActionFieldName.GameEffect
//                     ).value.transform.parent = customMono.rotationAndCenterObject.transform;
//                     GetActionField<ActionGameEffectField>(
//                         ActionFieldName.GameEffect
//                     ).value.transform.localPosition = Vector3.zero;

//                     break;
//                 }
//                 case 2: // fire punch
//                 {
//                     GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
//                         GetActionField<ActionCustomMonoField>(
//                             ActionFieldName.Target
//                         ).value.rotationAndCenterObject.transform.position
//                         - customMono.rotationAndCenterObject.transform.position;

//                     customMono.SetUpdateDirectionIndicator(
//                         GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
//                         UpdateDirectionIndicatorPriority.Low
//                     );
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
//                         GameManager.Instance.knockUpColliderPool.PickOne().gameEffect;

//                     var t_collideAndDamage = (CollideAndDamage)
//                         GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                             .value.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);

//                     t_collideAndDamage.allyTags = customMono.allyTags;
//                     t_collideAndDamage.collideDamage =
//                         GetActionField<ActionFloatField>(ActionFieldName.Damage).value * 0.5f;
//                     t_collideAndDamage.pushEnemyOnCollideForce = 20;
//                     GetActionField<ActionGameEffectField>(
//                         ActionFieldName.GameEffect
//                     ).value.transform.position = customMono.firePoint.transform.position;
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                         .value.rigidbody2D.AddForce(
//                             GetActionField<ActionVector3Field>(
//                                 ActionFieldName.Direction
//                             ).value.normalized * 5f,
//                             ForceMode2D.Impulse
//                         );
//                     break;
//                 }
//                 case 3: // forward kick
//                 {
//                     GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
//                         GetActionField<ActionCustomMonoField>(
//                             ActionFieldName.Target
//                         ).value.rotationAndCenterObject.transform.position
//                         - customMono.rotationAndCenterObject.transform.position;
//                     customMono.SetUpdateDirectionIndicator(
//                         GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
//                         UpdateDirectionIndicatorPriority.Low
//                     );
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
//                         GameManager.Instance.pushColliderPool.PickOne().gameEffect;

//                     var t_collideAndDamage = (CollideAndDamage)
//                         GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                             .value.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
//                     t_collideAndDamage.allyTags = customMono.allyTags;
//                     t_collideAndDamage.collideDamage =
//                         GetActionField<ActionFloatField>(ActionFieldName.Damage).value * 0.5f;
//                     t_collideAndDamage.pushEnemyOnCollideForce = 20;
//                     t_collideAndDamage.pushDirection = GetActionField<ActionVector3Field>(
//                         ActionFieldName.Direction
//                     ).value;
//                     GetActionField<ActionGameEffectField>(
//                         ActionFieldName.GameEffect
//                     ).value.transform.position = customMono.firePoint.transform.position;
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                         .value.rigidbody2D.AddForce(
//                             GetActionField<ActionVector3Field>(
//                                 ActionFieldName.Direction
//                             ).value.normalized * 5f,
//                             ForceMode2D.Impulse
//                         );
//                     break;
//                 }
//                 case 4: // Kame
//                 case 5: // left kick
//                 case 6: // punch
//                 {
//                     StartCoroutine(FireDragon());
//                     break;
//                 }
//                 case 7: // punch down
//                 {
//                     GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
//                         GetActionField<ActionCustomMonoField>(
//                             ActionFieldName.Target
//                         ).value.rotationAndCenterObject.transform.position
//                         - customMono.rotationAndCenterObject.transform.position;
//                     customMono.SetUpdateDirectionIndicator(
//                         GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
//                         UpdateDirectionIndicatorPriority.Low
//                     );
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
//                         GameManager.Instance.pushColliderPool.PickOne().gameEffect;

//                     var t_collideAndDamage = (CollideAndDamage)
//                         GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                             .value.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
//                     t_collideAndDamage.allyTags = customMono.allyTags;
//                     t_collideAndDamage.collideDamage =
//                         GetActionField<ActionFloatField>(ActionFieldName.Damage).value * 0.5f;
//                     t_collideAndDamage.pushEnemyOnCollideForce = 20;
//                     t_collideAndDamage.pushDirection = Vector2.down;
//                     GetActionField<ActionGameEffectField>(
//                         ActionFieldName.GameEffect
//                     ).value.transform.position = customMono.firePoint.transform.position;
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                         .value.rigidbody2D.AddForce(
//                             GetActionField<ActionVector3Field>(
//                                 ActionFieldName.Direction
//                             ).value.normalized * 5f,
//                             ForceMode2D.Impulse
//                         );
//                     break;
//                 }
//                 case 8: // swipe right
//                 {
//                     GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
//                         GetActionField<ActionCustomMonoField>(
//                             ActionFieldName.Target
//                         ).value.rotationAndCenterObject.transform.position
//                         - customMono.rotationAndCenterObject.transform.position;
//                     customMono.SetUpdateDirectionIndicator(
//                         GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
//                         UpdateDirectionIndicatorPriority.Low
//                     );
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
//                         GameManager.Instance.stunColliderPool.PickOne().gameEffect;

//                     var t_collideAndDamage = (CollideAndDamage)
//                         GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                             .value.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
//                     t_collideAndDamage.allyTags = customMono.allyTags;
//                     t_collideAndDamage.collideDamage =
//                         GetActionField<ActionFloatField>(ActionFieldName.Damage).value * 0.5f;
//                     t_collideAndDamage.stunDuration = 0.5f;
//                     GetActionField<ActionGameEffectField>(
//                         ActionFieldName.GameEffect
//                     ).value.transform.position = customMono.firePoint.transform.position;
//                     GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                         .value.rigidbody2D.AddForce(
//                             GetActionField<ActionVector3Field>(
//                                 ActionFieldName.Direction
//                             ).value.normalized * 5f,
//                             ForceMode2D.Impulse
//                         );
//                     break;
//                 }
//                 default:
//                     break;
//             }
//             #endregion
//         }

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill2)
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
//         ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         customMono.currentAction = null;
//     }

//     IEnumerator FireDragon()
//     {
//         CollideAndDamage t_collideAndDamage;

//         GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
//             GetActionField<ActionCustomMonoField>(
//                 ActionFieldName.Target
//             ).value.rotationAndCenterObject.transform.position
//             - customMono.rotationAndCenterObject.transform.position;
//         customMono.SetUpdateDirectionIndicator(
//             GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
//             UpdateDirectionIndicatorPriority.Low
//         );

//         for (int i = 0; i < 2; i++)
//         {
//             GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value = GameManager
//                 .Instance.phantomPulseDragonPool.PickOne()
//                 .gameEffect;

//             t_collideAndDamage = (CollideAndDamage)
//                 GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                     .value.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
//             t_collideAndDamage.allyTags = customMono.allyTags;
//             t_collideAndDamage.collideDamage = GetActionField<ActionFloatField>(
//                 ActionFieldName.Damage
//             ).value;
//             GetActionField<ActionGameEffectField>(
//                 ActionFieldName.GameEffect
//             ).value.transform.position = customMono.firePoint.transform.position;
//             GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
//                 .value.KeepFlyingAt(
//                     GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
//                     true,
//                     EasingType.OutQuint
//                 );

//             yield return new WaitForSeconds(0.2f);
//         }
//     }

//     void BotTrigger(Vector2 p_direction, float p_duration)
//     {
//         StartCoroutine(BotTriggerIE(p_direction, p_duration));
//     }

//     IEnumerator BotTriggerIE(Vector2 p_direction, float p_duration)
//     {
//         customMono.actionInterval = true;
//         Trigger(direction: p_direction);
//         yield return new WaitForSeconds(p_duration);
//         customMono.actionInterval = false;
//     }

//     public override void ActionInterrupt()
//     {
//         base.ActionInterrupt();
//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill2Signal, false);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
//     }
// }
