// using System.Collections;
// using UnityEngine;

// public class InfernalTide : SkillBase
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
//                 ActionUse.MeleeDamage,
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
//         GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
//         GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 5;
//         GetActionField<ActionFloatField>(ActionFieldName.Interval).value = 0.1f;
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 30f;
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
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
//             customMono.stat.might.FinalValue * 0.25f;
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
//             ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
//                     WaitSpawnFlame(direction)
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

//     IEnumerator WaitSpawnFlame(Vector3 p_direction)
//     {
//         customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(
//                 EAnimationSignal.MainSkill3Signal
//             )
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);
//         StartCoroutine(SpawnFlameIE(p_direction));

//         customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(
//                 EAnimationSignal.MainSkill3Signal
//             )
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);

//         transform.position -= p_direction.normalized * 1.5f;
//         CollideAndDamage t_fan = GameManager
//             .Instance.infernalTideFanPool.PickOne()
//             .gameEffect.GetCollideAndDamage();
//         t_fan.allyTags = customMono.allyTags;
//         t_fan.collideDamage = GetActionField<ActionFloatField>(ActionFieldName.Damage).value * 10;
//         t_fan.transform.SetPositionAndRotation(
//             customMono.firePoint.transform.position,
//             Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, p_direction))
//         );

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill3)
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
//         ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         customMono.currentAction = null;
//     }

//     IEnumerator SpawnFlameIE(Vector3 p_direction)
//     {
//         customMono.rotationAndCenterObject.transform.rotation = Quaternion.Euler(
//             customMono.rotationAndCenterObject.transform.rotation.eulerAngles.WithZ(
//                 Vector2.SignedAngle(Vector2.right, p_direction)
//             )
//         );

//         for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.EffectCount).value; i++)
//         {
//             SpawnNormalEffect(
//                 GameManager.Instance.infernalTideFlamePool.PickOneGameEffect(),
//                 customMono.rotationAndCenterObject.transform.TransformPoint(
//                     Vector3.right.WithY(i % 2 == 0 ? -i / 2 : (i + 1) / 2)
//                 ),
//                 p_isCombat: true
//             );

//             yield return new WaitForSeconds(
//                 GetActionField<ActionFloatField>(ActionFieldName.Interval).value
//             );
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
//         ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
//     }
// }
