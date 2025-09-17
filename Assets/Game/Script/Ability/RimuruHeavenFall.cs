// using System.Collections;
// using UnityEngine;

// public class RimuruHeavenFall : SkillBase
// {
//     public override void Awake()
//     {
//         base.Awake();
//     }

//     public override void OnEnable()
//     {
//         base.OnEnable();
//     }

//     public override void Start()
//     {
//         base.Start();
//         StatChangeRegister();
//     }

//     public override void Config()
//     {
//         GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 10f;
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 15f;
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         /* Also use damage, actionie */
//     }

//     public override void StatChangeRegister()
//     {
//         base.StatChangeRegister();
//         customMono.stat.wisdom.finalValueChangeEvent += RecalculateStat;
//     }

//     public override void RecalculateStat()
//     {
//         base.RecalculateStat();
//         GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
//             customMono.stat.wisdom.FinalValue * 0.1f;
//     }

//     public override void AddActionManuals()
//     {
//         base.AddActionManuals();
//         botActionManuals.Add(
//             new BotActionManual(
//                 ActionUse.RangedDamage,
//                 (p_doActionParamInfo) =>
//                     FireAt(
//                         p_doActionParamInfo.targetCenterPosition,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.5f)
//             )
//         );
//     }

//     public override ActionResult Trigger(
//         Vector2 p_location = default,
//         Vector2 p_direction = default
//     )
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
//             ToggleAnim(GameManager.Instance.mainSkill4BoolHash, true);
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
//                     TriggerCoroutine(p_location, p_direction)
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

//     IEnumerator TriggerCoroutine(Vector2 p_location = default, Vector2 p_direction = default)
//     {
//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(
//                 EAnimationSignal.MainSkill4Signal
//             )
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill4Signal, false);

//         SpawnNormalEffect(
//             GameManager.Instance.rimuruHeavenFallRayPool.PickOneGameEffect(),
//             p_location,
//             p_isCombat: true
//         );

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill4)
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill4, false);
//         ToggleAnim(GameManager.Instance.mainSkill4BoolHash, false);
//         customMono.currentAction = null;
//     }

//     public void FireAt(Vector2 location, float duration)
//     {
//         StartCoroutine(FireAtCoroutine(location, duration));
//     }

//     IEnumerator FireAtCoroutine(Vector2 location, float duration)
//     {
//         customMono.actionInterval = true;
//         Trigger(p_location: location);
//         yield return new WaitForSeconds(duration);

//         customMono.actionInterval = false;
//     }

//     public override void ActionInterrupt()
//     {
//         base.ActionInterrupt();
//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         ToggleAnim(GameManager.Instance.mainSkill4BoolHash, false);
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill4Signal, false);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill4, false);
//     }
// }
