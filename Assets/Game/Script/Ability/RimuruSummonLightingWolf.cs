// using System.Collections;
// using UnityEngine;

// public class RimuruSummonLightingWolf : SkillBase
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
//                         p_doActionParamInfo.firePointToTargetCenterDirection,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.5f)
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
//         GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 1f;
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 5f;
//         GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 2.95f;
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         /* Also use Damage */
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
//             customMono.stat.wisdom.FinalValue * 0.35f;
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
//         else if (canUse && !customMono.actionBlocking && !customMono.movementActionBlocking)
//         {
//             canUse = false;
//             customMono.actionBlocking = true;
//             customMono.movementActionBlocking = true;
//             ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
//                     WaitSpawnLightingWolf(direction)
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

//     IEnumerator WaitSpawnLightingWolf(Vector3 p_direction)
//     {
//         SpawnEffectAsChild(
//             Vector2.zero,
//             GameManager.Instance.rimuruLightingWolfSummonCirclePool.PickOneGameEffect()
//         );

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill3)
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         SpawnNormalEffect(
//             GameManager.Instance.rimuruLightingWolfPool.PickOneGameEffect(),
//             transform.position,
//             p_isCombat: true
//         );

//         yield return Flash(
//             Vector3.zero,
//             0,
//             GetActionField<ActionFloatField>(ActionFieldName.Duration).value
//         );

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
//         ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
//         customMono.actionBlocking = false;
//         customMono.movementActionBlocking = false;
//         customMono.currentAction = null;
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
//         customMono.movementActionBlocking = false;
//         ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
//     }
// }
