// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;

// public class Scatter : SkillBase
// {
//     ActionWaitInfo actionWaitInfo = new();
//     GameEffect scatterChargeGameEffect;
//     public static List<List<float>> arrowAnglesAtPhases;
//     SpriteRenderer scatterArrowPhaseIcon;
//     public static Vector3 punch;
//     public static float punchDuration = 0.5f,
//         elasticity = 1;
//     public int vibrato = 10;
//     Tweener iconTweener;

//     public override void Awake()
//     {
//         base.Awake();
//     }

//     public override void OnEnable()
//     {
//         base.OnEnable();
//         actionWaitInfo.stillWaiting = false;
//     }

//     public override void Start()
//     {
//         base.Start();
// #if UNITY_EDITOR
//         onExitPlayModeEvent += () => arrowAnglesAtPhases = null;
// #endif
//         StatChangeRegister();
//     }

//     public override void Config()
//     {
//         GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
//         audioClip = Resources.Load<AudioClip>("AudioClip/scatter-release");
//         actionWaitInfo.releaseBoolHash = Animator.StringToHash("Release");
//         /* In this skill ammo mean phase */
//         GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value = 0;
//         GetActionField<ActionIntField>(ActionFieldName.AllPhases).value = 3;
//         GetActionField<ActionFloatField>(ActionFieldName.Interval).value = 0.5f;

//         arrowAnglesAtPhases ??= new()
//         {
//             new List<float> { 0f.DegToRad() },
//             new List<float> { -30f.DegToRad(), 30f.DegToRad() },
//             new List<float> { 0f.DegToRad(), -30f.DegToRad(), 30f.DegToRad() },
//         };
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         scatterArrowPhaseIcon = transform
//             .Find("ScatterArrowPhaseIcon")
//             .GetComponent<SpriteRenderer>();

//         punch = new(0.2f, 0.2f);
//         punchDuration = 0.5f;
//         elasticity = 0;
//         vibrato = 10;
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 30f;
//         GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value = new();

//         /* Also use damage, actionIE */
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
//             customMono.stat.wisdom.FinalValue * 1.5f;
//     }

//     public override void AddActionManuals()
//     {
//         base.AddActionManuals();
//         botActionManuals.Add(
//             new BotActionManual(
//                 ActionUse.PushAway,
//                 (p_doActionParamInfo) => Trigger(),
//                 new(nextActionChoosingIntervalProposal: 1f),
//                 actionNeedWait: true,
//                 startAndWait: StartAndWait,
//                 whileWaiting: WhileWaiting
//             )
//         );
//     }

//     public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
//     {
//         if (actionWaitInfo.stillWaiting)
//         {
//             actionWaitInfo.stillWaiting = false;
//             return successResult;
//         }

//         return failResult;
//     }

//     public override ActionResult StartAndWait()
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
//             ToggleAnim(GameManager.Instance.chargeBoolHash, true);
//             actionWaitInfo.stillWaiting = true;
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
//                     WaitingCoroutine()
//             );
//             customMono.currentAction = this;
//             customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
//                 ActionFieldName.ManaCost
//             ).value;

//             return successResult;
//         }

//         return failResult;
//     }

//     IEnumerator WaitingCoroutine()
//     {
//         scatterChargeGameEffect = GameManager.Instance.scatterChargePool.PickOne().gameEffect;
//         scatterChargeGameEffect.Follow(transform);

//         GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Restart();
//         GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value = 0;
//         while (actionWaitInfo.stillWaiting)
//         {
//             yield return null;

//             if (
//                 GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value
//                 < GetActionField<ActionIntField>(ActionFieldName.AllPhases).value
//             )
//             {
//                 if (
//                     GetActionField<ActionStopWatchField>(
//                         ActionFieldName.StopWatch
//                     ).value.Elapsed.TotalSeconds
//                     > GetActionField<ActionFloatField>(ActionFieldName.Interval).value
//                 )
//                 {
//                     GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value++;
//                     HandleIcon();

//                     GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Restart();
//                 }
//             }
//         }

//         GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Stop();
//         scatterChargeGameEffect.deactivate();
//         StartCoroutine(CooldownCoroutine());
//         scatterArrowPhaseIcon.gameObject.SetActive(false);
//         iconTweener?.Kill();

//         if (GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value > 0)
//         {
//             ToggleAnim(actionWaitInfo.releaseBoolHash, true);
//             ToggleAnim(GameManager.Instance.chargeBoolHash, false);
//             customMono.audioSource.PlayOneShot(audioClip);

//             SpawnEffectAsChild(
//                 Vector2.zero,
//                 GameManager.Instance.scatterFlashPool.PickOneGameEffect()
//             );

//             arrowAnglesAtPhases[
//                 GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value - 1
//             ]
//                 .ForEach(arrowAngle =>
//                 {
//                     SetCombatProjectile(
//                         GameManager.Instance.scatterArrowPool.PickOneGameEffect(),
//                         actionWaitInfo.finalDirection.RotateZ(arrowAngle)
//                     );
//                 });

//             while (
//                 !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndRelease)
//             )
//                 yield return new WaitForSeconds(Time.fixedDeltaTime);

//             customMono.actionBlocking = false;
//             ToggleAnim(actionWaitInfo.releaseBoolHash, false);
//             customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndRelease, false);
//             customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         }
//         else
//         {
//             customMono.actionBlocking = false;
//             ToggleAnim(GameManager.Instance.chargeBoolHash, false);
//             customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         }

//         customMono.currentAction = null;
//     }

//     private void HandleIcon()
//     {
//         scatterArrowPhaseIcon.gameObject.SetActive(true);
//         switch (GetActionField<ActionIntField>(ActionFieldName.CurrentPhase).value)
//         {
//             case 1:
//             {
//                 scatterArrowPhaseIcon.transform.localScale = Vector3.one * 0.1f;
//                 scatterArrowPhaseIcon.color = ((Vector4)scatterArrowPhaseIcon.color).WithW(0.33f);
//                 scatterArrowPhaseIcon
//                     .transform.DOPunchScale(punch, punchDuration, vibrato, elasticity)
//                     .SetEase(Ease.OutQuart);
//                 // .OnComplete(() => spriteRenderer.enabled = false);
//                 break;
//             }
//             case 2:
//             {
//                 scatterArrowPhaseIcon.transform.localScale = Vector3.one * 0.1f;
//                 scatterArrowPhaseIcon.color = ((Vector4)scatterArrowPhaseIcon.color).WithW(0.66f);
//                 scatterArrowPhaseIcon
//                     .transform.DOPunchScale(punch, punchDuration, vibrato * 2, elasticity)
//                     .SetEase(Ease.OutQuart);
//                 // .OnComplete(() => spriteRenderer.enabled = false);
//                 break;
//             }
//             case 3:
//             {
//                 scatterArrowPhaseIcon.transform.localScale = Vector3.one * 0.1f;
//                 scatterArrowPhaseIcon.color = ((Vector4)scatterArrowPhaseIcon.color).WithW(1);
//                 iconTweener = scatterArrowPhaseIcon
//                     .transform.DOPunchScale(punch, punchDuration, vibrato * 3, elasticity)
//                     .SetEase(Ease.OutQuart)
//                     .SetLoops(-1);
//                 break;
//             }

//             default:
//                 break;
//         }
//     }

//     public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
//     {
//         customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
//         actionWaitInfo.finalDirection = p_direction;
//     }

//     public void ScatterTo(Vector2 direction, float duration)
//     {
//         StartCoroutine(ScatterToCoroutine(duration));
//     }

//     IEnumerator ScatterToCoroutine(float p_duration)
//     {
//         customMono.actionInterval = true;

//         yield return new WaitForSeconds(p_duration);
//         customMono.actionInterval = false;
//     }

//     public override void ActionInterrupt()
//     {
//         base.ActionInterrupt();
//         customMono.actionBlocking = false;
//         customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
//         ToggleAnim(GameManager.Instance.chargeBoolHash, false);
//         ToggleAnim(actionWaitInfo.releaseBoolHash, false);
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//         actionWaitInfo.stillWaiting = false;
//         GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Stop();
//         /* In case this is used somewhere we don't know*/
//         if (scatterChargeGameEffect.gameObject.activeSelf)
//             scatterChargeGameEffect.deactivate();
//         customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndRelease, false);

//         scatterArrowPhaseIcon.gameObject.SetActive(false);
//         iconTweener?.Kill();
//     }
// }
