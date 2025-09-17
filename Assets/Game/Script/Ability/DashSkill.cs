// using System.Collections;
// using Kryz.Tweening;
// using UnityEngine;

// public class DashSkill : SkillBase
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
//             new BotActionManual(
//                 ActionUse.GetCloser,
//                 (p_doActionParamInfo) =>
//                     DashTo(
//                         p_doActionParamInfo.centerToTargetCenterDirection,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.5f)
//             )
//         );
//         botActionManuals.Add(
//             new BotActionManual(
//                 ActionUse.GetAway,
//                 (p_doActionParamInfo) =>
//                     DashTo(
//                         p_doActionParamInfo.centerToTargetCenterDirection,
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(
//                     nextActionChoosingIntervalProposal: 0.5f,
//                     isDirectionModify: true,
//                     directionModifier: -1
//                 )
//             )
//         );
//         botActionManuals.Add(
//             new BotActionManual(
//                 ActionUse.Dodge,
//                 (p_doActionParamInfo) =>
//                     DashTo(
//                         new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
//                         p_doActionParamInfo.nextActionChoosingIntervalProposal
//                     ),
//                 new(nextActionChoosingIntervalProposal: 0.5f)
//             )
//         );
//     }

//     public override void Start()
//     {
//         base.Start();
//     }

//     public override void Config()
//     {
//         GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 10;
//         GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 0.5f;
//         GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 0.3f;
//         GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 8f;
//         GetActionField<ActionFloatField>(ActionFieldName.Interval).value =
//             GetActionField<ActionFloatField>(ActionFieldName.Duration).value
//             / GetActionField<ActionIntField>(ActionFieldName.EffectCount).value;
//         GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 15f;
//         successResult = new(
//             true,
//             ActionResultType.Cooldown,
//             GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
//         );
//         /* Also use current time */
//     }

//     public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
//     {
//         customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
//     }

//     public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
//     {
//         if (
//             customMono.stat.currentManaPoint.Value
//             < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
//         )
//             return failResult;
//         else if (canUse && !customMono.movementActionBlocking)
//         {
//             canUse = false;
//             customMono.movementActionBlocking = true;
//             StartCoroutine(
//                 GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = Dashing(
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

//     public IEnumerator Dashing(Vector3 p_direction)
//     {
//         SpawnEffectAsChild(Vector2.zero, GameManager.Instance.dashExplodePool.PickOneGameEffect());

//         StartCoroutine(SpawnAfterImage());
//         yield return Dash(
//             p_direction,
//             GetActionField<ActionFloatField>(ActionFieldName.Speed).value,
//             GetActionField<ActionFloatField>(ActionFieldName.Duration).value,
//             EasingFunctions.OutQuint
//         );

//         customMono.movementActionBlocking = false;
//         customMono.currentAction = null;
//     }

//     IEnumerator SpawnAfterImage()
//     {
//         GameEffect t_gameEffect;
//         for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.EffectCount).value; i++)
//         {
//             t_gameEffect = GameManager.Instance.dashEffectPool.PickOne().gameEffect;
//             t_gameEffect.animateObjects[0].spriteRenderer.sprite = customMono.spriteRenderer.sprite;
//             t_gameEffect.animateObjects[0].transform.localScale = customMono
//                 .directionModifier
//                 .transform
//                 .localScale;
//             t_gameEffect.transform.position = customMono.transform.position;

//             yield return new WaitForSeconds(
//                 GetActionField<ActionFloatField>(ActionFieldName.Interval).value
//             );
//         }
//     }

//     public void DashTo(Vector2 direction, float duration)
//     {
//         StartCoroutine(DashToCoroutine(direction, duration));
//     }

//     IEnumerator DashToCoroutine(Vector2 direction, float duration)
//     {
//         customMono.actionInterval = true;
//         Trigger(direction: direction);
//         yield return new WaitForSeconds(duration);

//         customMono.actionInterval = false;
//     }

//     public override void ActionInterrupt()
//     {
//         base.ActionInterrupt();
//         customMono.movementActionBlocking = true;
//         StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
//     }
// }
