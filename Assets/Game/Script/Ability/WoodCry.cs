// using System.Collections;
// using UnityEngine;

// public class WoodCry : SkillBase
// {
//     float healAmmount;

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
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 20f;
//         /* also damage, actionie */
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
//             customMono.stat.wisdom.FinalValue * 0.15f;
//         healAmmount = customMono.stat.wisdom.FinalValue * 0.07f;
//     }

//     public override void AddActionManuals()
//     {
//         base.AddActionManuals();
//         botActionManuals.Add(
//             new BotActionManual(
//                 ActionUse.RangedDamage,
//                 (p_doActionParamInfo) =>
//                     FireAt(
//                         p_doActionParamInfo.targetOriginPosition,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.5f)
//             )
//         );
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
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
//                     TriggerCoroutine(location, direction)
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

//     IEnumerator TriggerCoroutine(Vector2 location = default, Vector2 direction = default)
//     {
//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(
//                 EAnimationSignal.MainSkill2Signal
//             )
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill2Signal, false);

//         CollideAndDamage t_gameEffect =
//             GameManager
//                 .Instance.woodCryArrowPool.PickOne()
//                 .gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;

//         t_gameEffect.allyTags = customMono.allyTags;
//         t_gameEffect.collideDamage = GetActionField<ActionFloatField>(ActionFieldName.Damage).value;
//         t_gameEffect.healAmmount = healAmmount;
//         t_gameEffect.transform.position = location;

//         while (
//             !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill2)
//         )
//             yield return new WaitForSeconds(Time.fixedDeltaTime);

//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
//         ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
//         customMono.currentAction = null;
//     }

//     public void FireAt(Vector2 location, float duration)
//     {
//         StartCoroutine(FireAtCoroutine(location, duration));
//     }

//     IEnumerator FireAtCoroutine(Vector2 location, float duration)
//     {
//         customMono.actionInterval = true;
//         Trigger(location: location);
//         yield return new WaitForSeconds(duration);

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
