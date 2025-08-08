using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManagerRevamp : MonoEditorSingleton<GameUIManagerRevamp>
{
    Canvas canvas;
    public Button menuCharButton,
        menuConfigButton,
        characterScreenExitButton;
    public GameObject characterScreen,
        characterInfo,
        skillNodeUI,
        characterPartyNodePrefab,
        joystickZone,
        joystickPrefab,
        healthAndManaIndicatorPrefab,
        worldSpaceCanvas,
        tooltips,
        itemSkillTooltipPrefab,
        skillAndItemUseButtonsPrefab,
        skillAndItemUseZone;
    public RectTransform partyMenu;
    Dictionary<int, CharacterInfoUI> characterInfoUIDict = new();
    public CustomMono currentActiveCustomMono;
    ObjectPool healthAndManaIndicator;
    Vector2 screenTooltipRectSize;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuCharButton.onClick.AddListener(() =>
        {
            characterScreen.SetActive(true);
        });
        characterScreenExitButton.onClick.AddListener(() => characterScreen.SetActive(false));

        healthAndManaIndicatorPrefab = Resources.Load("HealthAndManaIndicator") as GameObject;
        healthAndManaIndicator = new(
            healthAndManaIndicatorPrefab,
            new PoolArgument(
                ComponentType.HealthAndManaIndicator,
                PoolArgument.WhereComponent.Self
            ),
            new PoolArgument(ComponentType.WorldSpaceUI, PoolArgument.WhereComponent.Self)
        );
        healthAndManaIndicator.handleCachedComponentRefs += (p_poolObject) =>
        {
            p_poolObject.gameObject.transform.SetParent(worldSpaceCanvas.transform, false);
        };
    }

    void GetAllUI() { }

    public override void Start()
    {
        base.Start();
    }

    Vector2 tooltipEndPosScaler = new Vector2(1, -1);
    Color transparentGreen = new Vector4(0, 1, 0, 0),
        transparentRed = new Vector4(1, 0, 0, 0);
    ActionResult actionResult;

    public void InitializeCharacterUI(CustomMono p_customMono)
    {
        currentActiveCustomMono ??= p_customMono;
        GameObject t_characterInfo = Instantiate(characterInfo);
        t_characterInfo.transform.SetParent(characterScreen.transform, false);
        t_characterInfo.transform.SetSiblingIndex(0);

        CharacterInfoUI t_characterInfoUI = t_characterInfo.GetComponent<CharacterInfoUI>();
        characterInfoUIDict.Add(p_customMono.GetHashCode(), t_characterInfoUI);
        t_characterInfo.SetActive(false);

        /* Joystick per character */
        t_characterInfoUI.enhancedOnScreenStick = Instantiate(joystickPrefab)
            .GetComponent<EnhancedOnScreenStick>();
        t_characterInfoUI.enhancedOnScreenStick.transform.SetParent(joystickZone.transform, false);
        t_characterInfoUI.enhancedOnScreenStick.gameObject.SetActive(false);
        t_characterInfoUI.enhancedOnScreenStick.OnMove = (vector2) =>
            p_customMono.movable.moveVector = vector2;

        /* Character avatar on main screen */
        t_characterInfoUI.characterPartyNode = Instantiate(characterPartyNodePrefab)
            .GetComponent<CharacterPartyNode>();
        t_characterInfoUI.characterPartyNode.transform.SetParent(partyMenu.transform, false);
        t_characterInfoUI.characterPartyNode.rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
        t_characterInfoUI.characterPartyNode.avatarIcon.sprite = p_customMono.charUIData.charImage;
        t_characterInfoUI.characterPartyNode.pointerDownEvent = (p_eventData) =>
        {
            if (currentActiveCustomMono != null)
            {
                /* This is what happen when we change character:
                - Change party menu selection
                - Show selected character info in character screen
                - Change camera target
                - Show corresponding joystick */
                CharacterInfoUI t_previousActiveCharInfoUI = characterInfoUIDict[
                    currentActiveCustomMono.GetHashCode()
                ];
                t_previousActiveCharInfoUI.characterPartyNode.rectTransform.localScale =
                    new Vector3(0.5f, 0.5f, 1);
                t_previousActiveCharInfoUI.gameObject.SetActive(false);
                t_previousActiveCharInfoUI.enhancedOnScreenStick.gameObject.SetActive(false);
                t_previousActiveCharInfoUI.skillAndItemUseButtons.gameObject.SetActive(false);
                currentActiveCustomMono.ResumeBot();
                currentActiveCustomMono = p_customMono;
                CharacterInfoUI t_currentActiveCharacterInfoUI = characterInfoUIDict[
                    currentActiveCustomMono.GetHashCode()
                ];
                t_currentActiveCharacterInfoUI.characterPartyNode.rectTransform.localScale =
                    new Vector3(1f, 1f, 1);
                t_currentActiveCharacterInfoUI.gameObject.SetActive(true);
                t_currentActiveCharacterInfoUI.enhancedOnScreenStick.gameObject.SetActive(true);
                t_currentActiveCharacterInfoUI.skillAndItemUseButtons.gameObject.SetActive(true);
                currentActiveCustomMono.PauseBot();
                GameManager.Instance.cinemachineCamera.Follow = p_customMono.transform;
                LayoutRebuilder.ForceRebuildLayoutImmediate(partyMenu);
            }
        };

        /* Item and skill use buttons on main screen*/
        GameObject t_skillAndItemUseButtons = Instantiate(skillAndItemUseButtonsPrefab);
        t_skillAndItemUseButtons.transform.SetParent(skillAndItemUseZone.transform, false);
        t_skillAndItemUseButtons.SetActive(false);
        t_characterInfoUI.skillAndItemUseButtons = t_skillAndItemUseButtons;
        t_characterInfoUI.skillUseUIs = t_skillAndItemUseButtons
            .GetComponentsInChildren<SkillUseUI>(true)
            .ToList();
        for (int i = 0; i < t_characterInfoUI.skillSlotUIs.Count; i++)
        {
            t_characterInfoUI.skillSlotUIs[i].skillUseUI = t_characterInfoUI.skillUseUIs[i];
        }

        #region Skill Slot
        /* Handle skill slot UI:
        - When clicked, check if any skill is in queue, if so, equip that skill in
        the slot and remove it from the queue.
        - Also when a skill is equipped, show it in the main screen where player
        can interact with it. */
        t_characterInfoUI.skillSlotUIs.ForEach(t_skillSlotUI =>
        {
            t_skillSlotUI.pointerDownEvent += (p_eventData) =>
            {
                if (t_characterInfoUI.queueSkillNodeUI != null)
                {
                    /* If the node is already equipped, remove it from equipped slot first */
                    if (t_characterInfoUI.queueSkillNodeUI.equippedSlot != null)
                    {
                        t_characterInfoUI.queueSkillNodeUI.equippedSlot.icon.gameObject.SetActive(
                            false
                        );
                        /* Remove use from main screen */
                        /* Reset skill use ui event */
                        t_characterInfoUI.queueSkillNodeUI.equippedSlot.skillUseUI.ResetEvent();
                        t_characterInfoUI.queueSkillNodeUI.equippedSlot.skillUseUI.gameObject.SetActive(
                            false
                        );
                        t_characterInfoUI.queueSkillNodeUI.equippedSlot.skillNodeUI = null;
                        t_characterInfoUI.queueSkillNodeUI.equippedSlot = null;
                    }

                    /* If the the slot is already occupied, remove the current skill in the current slot first */
                    if (t_skillSlotUI.skillNodeUI != null)
                    {
                        t_skillSlotUI.skillUseUI.ResetEvent();
                        t_skillSlotUI.skillUseUI.gameObject.SetActive(false);
                        t_skillSlotUI.skillNodeUI.equippedSlot = null;
                        t_skillSlotUI.skillNodeUI = null;
                    }

                    t_skillSlotUI.border.color = Color.green;
                    t_skillSlotUI.border.DOColor(transparentGreen, 1).SetEase(Ease.OutQuart);
                    t_skillSlotUI.icon.gameObject.SetActive(true);
                    t_skillSlotUI.icon.sprite = t_characterInfoUI.queueSkillNodeUI.icon.sprite;

                    t_characterInfoUI.queueSkillNodeUI.equippedSlot = t_skillSlotUI;
                    t_skillSlotUI.skillNodeUI = t_characterInfoUI.queueSkillNodeUI;
                    t_skillSlotUI.skillUseUI.icon.image.sprite = t_skillSlotUI.icon.sprite;
                    t_skillSlotUI.skillUseUI.gameObject.SetActive(true);

                    t_skillSlotUI.skillUseUI.skillIndicatorType = t_characterInfoUI
                        .queueSkillNodeUI
                        .skillDataSO
                        .skillIndicatorType;
                    switch (t_characterInfoUI.queueSkillNodeUI.skillDataSO.inputType)
                    {
                        case SkillDataSO.InputType.Click:
                        {
                            t_skillSlotUI.skillUseUI.pointerDownEvent = (
                                p_pointerPosition,
                                p_centerToPointerDir
                            ) =>
                            {
                                actionResult = t_skillSlotUI.skillNodeUI.skillBase.Trigger(
                                    p_location: p_pointerPosition,
                                    p_direction: p_centerToPointerDir
                                );

                                switch (actionResult.actionResultType)
                                {
                                    case ActionResultType.Cooldown:
                                    {
                                        t_skillSlotUI.skillUseUI.StartCooldown(actionResult);
                                        break;
                                    }
                                    case ActionResultType.AdditionalPhaseWithCondition:
                                    {
                                        t_skillSlotUI.skillUseUI.StartAdditionalPhaseWithCondition(
                                            actionResult
                                        );
                                        break;
                                    }
                                    default:
                                        break;
                                }
                            };
                            break;
                        }
                        case SkillDataSO.InputType.Hold:
                        {
                            t_skillSlotUI.skillUseUI.holdEvent = (
                                p_pointerPosition,
                                p_centerToPointerDir
                            ) =>
                            {
                                actionResult = t_skillSlotUI.skillNodeUI.skillBase.Trigger(
                                    p_location: p_pointerPosition,
                                    p_direction: p_centerToPointerDir
                                );

                                switch (actionResult.actionResultType)
                                {
                                    case ActionResultType.Cooldown:
                                    {
                                        t_skillSlotUI.skillUseUI.StartCooldown(actionResult);
                                        break;
                                    }
                                    case ActionResultType.AdditionalPhaseWithCondition:
                                    {
                                        t_skillSlotUI.skillUseUI.StartAdditionalPhaseWithCondition(
                                            actionResult
                                        );
                                        break;
                                    }
                                    default:
                                        break;
                                }
                            };
                            break;
                        }
                        case SkillDataSO.InputType.HoldAndRelease:
                        {
                            t_skillSlotUI.skillUseUI.pointerDownEvent = (
                                p_pointerPosition,
                                p_centerToPointerDir
                            ) => t_skillSlotUI.skillNodeUI.skillBase.StartAndWait();

                            t_skillSlotUI.skillUseUI.holdEvent = (
                                p_pointerPosition,
                                p_centerToPointerDir
                            ) =>
                                t_skillSlotUI.skillNodeUI.skillBase.WhileWaiting(
                                    p_location: p_pointerPosition,
                                    p_direction: p_centerToPointerDir
                                );

                            t_skillSlotUI.skillUseUI.pointerUpEvent = (
                                p_pointerPosition,
                                p_centerToPointerDir
                            ) =>
                            {
                                actionResult = t_skillSlotUI.skillNodeUI.skillBase.Trigger(
                                    p_location: p_pointerPosition,
                                    p_direction: p_centerToPointerDir
                                );

                                switch (actionResult.actionResultType)
                                {
                                    case ActionResultType.Cooldown:
                                    {
                                        t_skillSlotUI.skillUseUI.StartCooldown(actionResult);
                                        break;
                                    }
                                    case ActionResultType.AdditionalPhaseWithCondition:
                                    {
                                        t_skillSlotUI.skillUseUI.StartAdditionalPhaseWithCondition(
                                            actionResult
                                        );
                                        break;
                                    }
                                    default:
                                        break;
                                }
                            };
                            break;
                        }
                        default:
                            break;
                    }

                    t_characterInfoUI.queueSkillNodeUI = null;
                }
            };
        });
        #endregion

        #region Load Skill UI
        /* Load Skill UI */
        p_customMono.skill.skillDataSOs.ForEach(t_skillDataSO =>
        {
            GameObject t_skillNodeUI = Instantiate(skillNodeUI);
            t_skillNodeUI.transform.SetParent(t_characterInfoUI.skillSVContent.transform, false);

            SkillNodeUI t_skillNodeUIComp = t_skillNodeUI.GetComponent<SkillNodeUI>();
            t_skillNodeUIComp.icon.sprite = t_skillDataSO.skillImage;
            t_skillNodeUIComp.skillDataSO = t_skillDataSO;
            t_skillNodeUIComp.skillBase =
                p_customMono.GetComponent(Type.GetType(t_skillDataSO.skillName)) as SkillBase;

            TooltipUI t_skillTooltip = Instantiate(itemSkillTooltipPrefab)
                .GetComponent<TooltipUI>();
            if (screenTooltipRectSize == Vector2.zero)
                screenTooltipRectSize = t_skillTooltip.rectTransform.rect.size * canvas.scaleFactor;
            t_skillTooltip.textName.text = t_skillDataSO.skillName;
            t_skillTooltip.textDescription.text = t_skillDataSO.skillHelperDescription;
            t_skillTooltip.transform.SetParent(t_characterInfoUI.tooltips.transform, false);
            t_skillTooltip.gameObject.SetActive(false);
            t_skillTooltip.equipButton.pointerDownEvent += (p_eventData) =>
            {
                t_characterInfoUI.skillSlotUIs.ForEach(t_skillSlotUI =>
                {
                    t_skillSlotUI.border.color = Color.green;
                    t_skillSlotUI.border.DOColor(transparentGreen, 1).SetEase(Ease.OutQuart);
                    t_skillTooltip.gameObject.SetActive(false);
                    t_characterInfoUI.queueSkillNodeUI = t_skillNodeUIComp;
                });
            };

            t_skillTooltip.unequipButton.pointerDownEvent += (p_eventData) =>
            {
                if (t_skillNodeUIComp.equippedSlot != null)
                {
                    t_skillNodeUIComp.equippedSlot.border.color = Color.red;
                    t_skillNodeUIComp
                        .equippedSlot.border.DOColor(transparentRed, 1)
                        .SetEase(Ease.OutQuart);
                    t_skillTooltip.gameObject.SetActive(false);
                    t_skillNodeUIComp.equippedSlot.icon.gameObject.SetActive(false);
                    t_skillNodeUIComp.equippedSlot.skillUseUI.ResetEvent();
                    t_skillNodeUIComp.equippedSlot.skillUseUI.gameObject.SetActive(false);
                    t_skillNodeUIComp.equippedSlot.skillNodeUI = null;
                    t_skillNodeUIComp.equippedSlot = null;
                }
            };

            t_skillNodeUIComp.pointerDownEvent += (p_eventData) =>
            {
                t_skillTooltip.gameObject.SetActive(true);
                t_skillTooltip.gameObject.transform.SetAsLastSibling();
                Vector2 t_endPosition =
                        p_eventData.position + tooltipEndPosScaler * screenTooltipRectSize,
                    t_tooltipPosition;

                t_tooltipPosition = p_eventData.position;
                if (t_endPosition.x > Screen.width)
                    t_tooltipPosition.x -= t_endPosition.x - Screen.width;
                if (t_endPosition.y < 0)
                    t_tooltipPosition.y += -t_endPosition.y;
                t_skillTooltip.transform.position = t_tooltipPosition;
            };
        });
        #endregion
    }

    public PoolObject CreateAndHandleHPAndMPUIWithFollowing(Transform transform)
    {
        PoolObject healthAndManaIndicatorObj = healthAndManaIndicator.PickOne();
        healthAndManaIndicatorObj.worldSpaceUI.FollowSlowly(transform);
        return healthAndManaIndicatorObj;
    }
}
