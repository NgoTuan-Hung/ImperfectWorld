// using System.Collections;
// using UnityEngine;

// public class GetOverThere : SkillBase
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
//                         p_doActionParamInfo.targetOriginPosition,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.4f)
//             )
//         );
//     }

//     public override void Start()
//     {
//         base.Start();
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
//         if (canUse && !customMono.actionBlocking)
//         {
//             canUse = false;
//             customMono.actionBlocking = true;
//             customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
//             ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
//                     WaitSpawnBlueHole(location)
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

//     IEnumerator WaitSpawnBlueHole(Vector3 p_location)
//     {
//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(
//                 EAnimationSignal.MainSkill2Signal
//             )
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill2Signal, false);
//         BlueHole blueHole = (BlueHole)
//             GameManager
//                 .Instance.blueHolePool.PickOne()
//                 .gameEffect.GetBehaviour(EGameEffectBehaviour.BlueHole);
//         blueHole.allyTags = customMono.allyTags;
//         blueHole.transform.position = p_location;

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

//     void BotTrigger(Vector2 p_location, float p_duration)
//     {
//         StartCoroutine(BotTriggerIE(p_location, p_duration));
//     }

//     IEnumerator BotTriggerIE(Vector2 p_location, float p_duration)
//     {
//         customMono.actionInterval = true;
//         Trigger(location: p_location);
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
