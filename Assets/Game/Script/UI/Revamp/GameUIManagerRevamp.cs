using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManagerRevamp : MonoEditorSingleton<GameUIManagerRevamp>
{
    Canvas canvas;
    public Button menuCharButton,
        characterScreenExitButton;
    public GameObject characterScreen,
        characterInfo,
        skillNodeUI,
        characterPartyNodePrefab,
        joystickZone,
        joystickPrefab,
        healthBarPrefab,
        worldSpaceCanvas,
        tooltips,
        itemSkillTooltipPrefab;
    public RectTransform partyMenu;
    Dictionary<int, CharacterInfoUI> characterInfoUIDict = new();
    public CustomMono currentActiveCustomMono;
    ObjectPool healthBarPool;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuCharButton.onClick.AddListener(() =>
        {
            characterScreen.SetActive(true);
        });
        characterScreenExitButton.onClick.AddListener(() => characterScreen.SetActive(false));

        healthBarPrefab = Resources.Load("HealthBar") as GameObject;
        healthBarPool = new(
            healthBarPrefab,
            20,
            new PoolArgument(ComponentType.HealthUIRevamp, PoolArgument.WhereComponent.Self),
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        healthBarPool.ForEach(
            (p_pO) =>
            {
                p_pO.gameObject.transform.SetParent(worldSpaceCanvas.transform, false);
            }
        );
    }

    void GetAllUI() { }

    public override void Start()
    {
        base.Start();
    }

    public void InitializeCharacterUI(CustomMono p_customMono)
    {
        currentActiveCustomMono ??= p_customMono;
        GameObject t_characterInfo = Instantiate(characterInfo);
        t_characterInfo.transform.SetParent(characterScreen.transform, false);
        t_characterInfo.transform.SetSiblingIndex(0);

        CharacterInfoUI t_characterInfoUI = t_characterInfo.GetComponent<CharacterInfoUI>();
        characterInfoUIDict.Add(p_customMono.GetHashCode(), t_characterInfoUI);
        t_characterInfo.SetActive(false);

        t_characterInfoUI.enhancedOnScreenStick = Instantiate(joystickPrefab)
            .GetComponent<EnhancedOnScreenStick>();
        t_characterInfoUI.enhancedOnScreenStick.transform.SetParent(joystickZone.transform, false);
        t_characterInfoUI.enhancedOnScreenStick.gameObject.SetActive(false);
        t_characterInfoUI.enhancedOnScreenStick.OnMove = (vector2) =>
            p_customMono.movable.moveVector = vector2;

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
                currentActiveCustomMono.ResumeBot();
                currentActiveCustomMono = p_customMono;
                CharacterInfoUI t_currentActiveCharacterInfoUI = characterInfoUIDict[
                    currentActiveCustomMono.GetHashCode()
                ];
                t_currentActiveCharacterInfoUI.characterPartyNode.rectTransform.localScale =
                    new Vector3(1f, 1f, 1);
                t_currentActiveCharacterInfoUI.gameObject.SetActive(true);
                t_currentActiveCharacterInfoUI.enhancedOnScreenStick.gameObject.SetActive(true);
                currentActiveCustomMono.PauseBot();
                GameManager.Instance.cinemachineCamera.Follow = p_customMono.transform;
                LayoutRebuilder.ForceRebuildLayoutImmediate(partyMenu);
            }
        };

        /* Load Skill UI */
        p_customMono.skill.skillDataSOs.ForEach(t_skillDataSO =>
        {
            GameObject t_skillNodeUI = Instantiate(skillNodeUI);
            t_skillNodeUI.transform.SetParent(t_characterInfoUI.skillSVContent.transform, false);

            SkillNodeUI t_skillNodeUIComp = t_skillNodeUI.GetComponent<SkillNodeUI>();
            t_skillNodeUIComp.icon.sprite = t_skillDataSO.skillImage;

            TooltipUI t_skillTooltip = Instantiate(itemSkillTooltipPrefab)
                .GetComponent<TooltipUI>();
            t_skillTooltip.transform.SetParent(t_characterInfoUI.tooltips.transform, false);
            t_skillTooltip.gameObject.SetActive(false);
            t_skillNodeUIComp.pointerDownEvent += (p_eventData) =>
            {
                t_skillTooltip.gameObject.SetActive(true);
                t_skillTooltip.transform.position = p_eventData.position;
            };
        });
    }

    public PoolObject CreateAndHandleRadialProgressFollowing(Transform transform)
    {
        PoolObject healthBarObject = healthBarPool.PickOne();
        healthBarObject.gameEffect.FollowSlowly(transform);
        return healthBarObject;
    }
}
