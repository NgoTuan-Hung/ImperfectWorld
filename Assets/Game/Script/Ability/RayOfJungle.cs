// using System.Collections;
// using UnityEngine;

// public class RayOfJungle : SkillBase
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
//                 ActionUse.RangedDamage,
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
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 15f;
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         /* Also use actionie , damage*/
//     }

//     public override void StatChangeRegister()
//     {
//         base.StatChangeRegister();
//         customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
//     }

//     public override void RecalculateStat()
//     {
//         base.RecalculateStat();
//         GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
//             customMono.stat.reflex.FinalValue * 0.11f;
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
//             ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
//                     location,
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

//     IEnumerator TriggerIE(Vector2 p_location = default, Vector2 p_direction = default)
//     {
//         customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(
//                 EAnimationSignal.MainSkill1Signal
//             )
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);
//         SpawnEffectAsChild(
//             p_direction,
//             GameManager.Instance.rayOfJungleBeamPool.PickOneGameEffect()
//         );

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
//         customMono.actionBlocking = false;
//         ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         customMono.currentAction = null;
//     }

//     void BotTrigger(Vector2 p_direction, float p_duration)
//     {
//         StartCoroutine(botIE = BotTriggerIE(p_direction, p_duration));
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
//         ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//     }
// }
