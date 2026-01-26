using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Map;
using TMPEffects.Components;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneMode
{
    MainMenu,
    MainGame,
}

public partial class GameUIManager : MonoSingleton<GameUIManager>
{
    List<RestSiteHealSelector> restSiteHealSelectors = new();
    ObjectPool restSiteHealSelectorPool;
    public SceneMode sceneMode = SceneMode.MainMenu;
    private static WaitForSeconds _waitForSeconds1 = new(1f);
    Canvas canvas;
    public Button menuCharButton,
        menuMapButton,
        menuInventoryButton,
        menuSettingButton;
    TextMeshProUGUI helperTextTMP;
    public GameObject mainScreen,
        joystickZone,
        joystickPrefab,
        healthAndManaIndicatorPrefab,
        worldSpaceCanvas,
        mapBackground,
        champUIZone,
        inventory,
        inventoryContent,
        freeZone,
        menu,
        upgradeZone,
        championRewardSelectZone,
        traderZone,
        buyZone,
        championWare,
        itemWare,
        relic,
        relicContent,
        eventContainer;
    ObjectPool healthAndManaIndicator,
        textPopupUIPool;
    public MapViewUI mapViewUI;
    public InteractiveButtonUI gameInteractionButton,
        restSiteHealOneButton,
        restSiteHealAllButton;
    public TextMeshProUGUI gameInteractionButtonTMP;
    GameObject champInfoPanel;
    Dictionary<CustomMono, ChampInfoPanel> champInfoPanelDict = new();
    Vector3 cameraMoveVector;
    public float cameraMovementSpeed = 0.1f;
    public GameObject cameraFollowObject;
    public float planeDistance = 5f,
        worldSpacePlaneDistance;
    public GameObject inventorySlotPF;
    public List<GameObject> inventorySlots = new();
    int inventorySlotCount = 0;
    public List<Item> playerItemUIs = new();
    public Vector2 ItemInventoryAnchorPos = new(0.5f, 0.5f);
    bool mapState = true;
    public List<StatUpgradeUI> statUpgrades;
    public List<ChampionRewardUI> championRewardUIs;
    bool finishedReward = false;
    bool menuCharStateOn = false;
    bool menuItemStateOn = false;
    public Image screenEffectImg,
        eventEffect,
        storyImage;
    public Animator screenEffectAnimator;
    TMPAnimator helperTextTMPA;
    TMPWriter storyText;
    public Vector2[] itemRewardPos = new Vector2[3];
    public Vector2[] championRewardPos = new Vector2[3];
    List<Item> itemRewards;
    List<TraderWare> championWares,
        itemWares;
    PlayerGold playerGold;
    public DialogBox guideDialogBox,
        campfireDialogBox;
    public UIWithEffect eventZone,
        eventImage;
    MysteryEventDescription mysteryEventDescription;
    List<EventChoiceButton> eventChoices;
    public GameObject doubleTapTooltipPrefab;
    PointerDownUI story,
        storySkip;
    public StoryPageSO currentStoryPage;

    private void Awake()
    {
        // ScrollView
        canvas = GetComponent<Canvas>();

        if (sceneMode == SceneMode.MainMenu) { }
        else
        {
            InitPrefabAndPool();
            FindChilds();
            Init();
            RegisterEvents();
        }
    }

    private void RegisterEvents()
    {
        story.pointerDownEvent += ProgressStory;
        storySkip.pointerDownEvent += CloseStory;
        menuCharButton.onClick.AddListener(ClickMenuCharButton);
        menuMapButton.onClick.AddListener(ClickMenuMapButton);
        menuInventoryButton.onClick.AddListener(ClickMenuItemButton);
        guideDialogBox.pointerDownEvent += CloseGuideDialog;
        mysteryEventDescription.onFinishWriter += ShowEventChoices;
        eventZone.onComplete += WriteEventDescription;
        restSiteHealOneButton.pointerClickEvent += ShowAllRestSiteHealSelectors;
        restSiteHealAllButton.pointerClickEvent += SelectRestSiteHealAll;
    }

    void ClickMenuCharButton()
    {
        if (menuCharStateOn)
        {
            foreach (var kvp in champInfoPanelDict)
            {
                kvp.Value.OnTabClick(kvp.Value.xIPTB);
            }
            menuCharStateOn = false;
        }
        else
        {
            foreach (var kvp in champInfoPanelDict)
            {
                kvp.Value.ShowIfOwnerIsActivated();
                kvp.Value.OnTabClick(kvp.Value.statIPTB);
            }
            menuCharStateOn = true;
        }
    }

    void ClickMenuMapButton()
    {
        if (mapState)
        {
            TurnOnMap();
        }
        else
        {
            if (GameManager.Instance.gameState == GameState.MapTravelingPhase)
                return;
            TurnOffMap();
        }

        mapState = !mapState;
    }

    void ClickMenuItemButton()
    {
        if (menuItemStateOn)
        {
            inventory.SetActive(false);
            foreach (var item in champInfoPanelDict)
            {
                item.Value.OnTabClick(item.Value.xIPTB);
            }
            menuItemStateOn = false;
        }
        else
        {
            inventory.SetActive(true);
            foreach (var item in champInfoPanelDict)
            {
                item.Value.ShowIfOwnerIsActivated();
                item.Value.OnTabClick(item.Value.itemIPTB);
            }
            menuItemStateOn = true;
        }
    }

    public void ActivateAllChampInfoPanelGlassEffect()
    {
        foreach (var item in champInfoPanelDict)
        {
            item.Value.ActivateGlassEffect();
        }
    }

    public void DeactivateAllChampInfoPanelGlassEffect()
    {
        foreach (var item in champInfoPanelDict)
        {
            item.Value.DeactivateGlassEffect();
        }
    }

    private void FindChilds()
    {
        mainScreen = transform.Find("MainScreen").gameObject;
        mapBackground = transform.Find("MapBackground").gameObject;
        inventoryContent = inventory.transform.Find("Viewport/Content").gameObject;
        menu = transform.Find("Menu").gameObject;
        menuCharButton = menu.transform.Find("MenuCharButton").GetComponent<Button>();
        menuMapButton = menu.transform.Find("MenuMapButton").GetComponent<Button>();
        menuInventoryButton = menu.transform.Find("MenuInventoryButton").GetComponent<Button>();
        menuSettingButton = menu.transform.Find("MenuSettingButton").GetComponent<Button>();
        playerGold = menu.transform.Find("PlayerGold").GetComponent<PlayerGold>();
        upgradeZone = transform.Find("MainScreen/UpgradeZone").gameObject;
        statUpgrades = upgradeZone.transform.GetComponentsInChildren<StatUpgradeUI>(true).ToList();
        helperTextTMP = transform.Find("MainScreen/HelperText").GetComponent<TextMeshProUGUI>();
        helperTextTMPA = helperTextTMP.GetComponent<TMPAnimator>();
        championRewardSelectZone = transform.Find("MainScreen/ChampionRewardSelectZone").gameObject;
        traderZone = worldSpaceCanvas.transform.Find("TraderZone").gameObject;
        buyZone = traderZone.transform.Find("BuyZone").gameObject;
        championWare = traderZone.transform.Find("ChampionWare").gameObject;
        championWares = championWare.GetComponentsInChildren<TraderWare>(true).ToList();
        itemWare = traderZone.transform.Find("ItemWare").gameObject;
        itemWares = itemWare.GetComponentsInChildren<TraderWare>(true).ToList();
        gameInteractionButtonTMP = gameInteractionButton.GetComponentInChildren<TextMeshProUGUI>();
        eventZone = transform.Find("MainScreen/EventZone").GetComponent<UIWithEffect>();
        eventContainer = eventZone.transform.Find("EventContainer").gameObject;
        eventEffect = eventZone.transform.Find("EventEffect").GetComponent<Image>();
        eventImage = eventContainer.transform.Find("EventImage").GetComponent<UIWithEffect>();
        mysteryEventDescription = eventContainer
            .transform.Find("EventDescription")
            .GetComponent<MysteryEventDescription>();
        eventChoices = eventContainer
            .transform.GetComponentsInChildren<EventChoiceButton>(true)
            .ToList();
        relic = transform.Find("MainScreen/Relic").gameObject;
        relicContent = relic.transform.Find("Viewport/Content").gameObject;
        story = transform.Find("Story").GetComponent<PointerDownUI>();
        storyImage = story.transform.Find("StoryImage").GetComponent<Image>();
        storyText = story.transform.Find("StoryText").GetComponent<TMPWriter>();
        storySkip = story.transform.Find("StorySkip").GetComponent<PointerDownUI>();
        restSiteHealOneButton = campfireDialogBox
            .transform.Find("RestSiteHealOneButton")
            .GetComponent<InteractiveButtonUI>();
        restSiteHealAllButton = campfireDialogBox
            .transform.Find("RestSiteHealAllButton")
            .GetComponent<InteractiveButtonUI>();

        /* Temp */
        for (int i = 0; i < inventoryContent.transform.childCount; i++)
        {
            inventorySlots.Add(inventoryContent.transform.GetChild(i).gameObject);
        }
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        var newPos = cameraFollowObject.transform.position + cameraMoveVector;
        if (GameManager.Instance.CheckInsideGlobalWall(newPos))
            cameraFollowObject.transform.position = newPos;
    }

    void InitPrefabAndPool()
    {
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

        textPopupUIPool = new(
            Resources.Load("TextPopupUI") as GameObject,
            new PoolArgument(ComponentType.TextPopupUI, PoolArgument.WhereComponent.Self)
        );
        textPopupUIPool.handleCachedComponentRefs += (p_pO) =>
            p_pO.gameObject.transform.SetParent(worldSpaceCanvas.transform, false);

        champInfoPanel = Resources.Load("UI/ChampInfoPanel") as GameObject;
        restSiteHealSelectorPool = new(
            Resources.Load("RestSiteHealSelector") as GameObject,
            new PoolArgument(ComponentType.NPC, PoolArgument.WhereComponent.Self)
        );
    }

    public void Init()
    {
        /* Joystick per character */
        var joystick = Instantiate(joystickPrefab).GetComponent<EnhancedOnScreenStick>();
        joystick.transform.SetParent(joystickZone.transform, false);

        cameraFollowObject = new GameObject();
        cameraFollowObject.name = "CameraFollowObject";
        cameraFollowObject.transform.position = Vector3.zero;
        GameManager.Instance.cinemachineCamera.Target.TrackingTarget = cameraFollowObject.transform;
        joystick.OnMove = (vector2) =>
        {
            cameraMoveVector = vector2.AsVector3() * cameraMovementSpeed;
        };

        canvas.planeDistance = planeDistance;
        worldSpacePlaneDistance = Math.Abs(
            GameManager.Instance.cinemachineCamera.transform.localScale.z
        );
        mapBackground.SetActive(true);
        story.gameObject.SetActive(true);
    }

    public PoolObject CreateAndHandleHPAndMPUIWithFollowing(Transform transform)
    {
        PoolObject healthAndManaIndicatorObj = healthAndManaIndicator.PickOne();
        healthAndManaIndicatorObj.WorldSpaceUI.FollowSlowly(transform);
        return healthAndManaIndicatorObj;
    }

    public void ReHandleHPAndMPUIWithFollowing(PoolObject poolObject, Transform transform)
    {
        poolObject.WorldSpaceUI.FollowSlowly(transform);
    }

    public PoolObject PickOneTextPopupUI() => textPopupUIPool.PickOne();

    public void TurnOnMap()
    {
        mapBackground.SetActive(true);
        mapViewUI.GetScrollRectForMap().gameObject.SetActive(true);
    }

    public void UnlockMap() => MapPlayerTracker.Instance.Unlock();

    public void TurnOffMap()
    {
        mapBackground.SetActive(false);
        mapViewUI.GetScrollRectForMap().gameObject.SetActive(false);
    }

    public void GenerateAndBindChampUI(CustomMono p_customMono)
    {
        var t_champUIPanel = Instantiate(champInfoPanel).GetComponent<ChampInfoPanel>();
        t_champUIPanel.transform.SetParent(champUIZone.transform, false);
        t_champUIPanel.Init(p_customMono);
        champInfoPanelDict[p_customMono] = t_champUIPanel;
        t_champUIPanel.gameObject.SetActive(false);
    }

    public void DestroyChampUI(CustomMono p_customMono)
    {
        var t_champUIPanel = champInfoPanelDict[p_customMono];
        champInfoPanelDict.Remove(p_customMono);
        Destroy(t_champUIPanel.gameObject);
    }

    public void ShowChampUI(CustomMono p_customMono) =>
        champInfoPanelDict[p_customMono].gameObject.SetActive(true);

    public ChampInfoPanel GetChampInfoPanel(CustomMono p_customMono) =>
        champInfoPanelDict[p_customMono];

    public void RemoveFromInventory(Item itemUI)
    {
        itemUI.transform.parent.SetAsLastSibling();
        /* The order in the list must be synced with the order in the UI hierarchy */
        inventorySlots.Remove(itemUI.transform.parent.gameObject);
        inventorySlots.Add(itemUI.transform.parent.gameObject);
        itemUI.transform.SetParent(freeZone.transform);
        playerItemUIs.Remove(itemUI);
    }

    public void AddToInventory(Item itemUI)
    {
        int selectedInventorySlot = -1;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].transform.childCount == 0)
            {
                selectedInventorySlot = i;
                break;
            }
        }

        if (selectedInventorySlot == -1)
        {
            var newInvSlot = Instantiate(inventorySlotPF);
            newInvSlot.transform.SetParent(inventoryContent.transform, false);
            inventorySlots.Add(newInvSlot);
            selectedInventorySlot = inventorySlots.Count - 1;
        }

        itemUI.transform.SetParent(inventorySlots[selectedInventorySlot].transform, false);
        itemUI.rectTransform.anchorMin = ItemInventoryAnchorPos;
        itemUI.rectTransform.anchorMax = ItemInventoryAnchorPos;
        itemUI.rectTransform.anchoredPosition = Vector2.zero;
        playerItemUIs.Add(itemUI);
    }

    public void SpawnReward()
    {
        StartCoroutine(SpawnRewardIE());
    }

    IEnumerator SpawnRewardIE()
    {
        helperTextTMP.gameObject.SetActive(true);
        helperTextTMP.text = "<+pivot duration=1><wave><palette>select one champion !</+>";

        yield return SpawnChampionRewardIE();

        helperTextTMP.text = "<+pivot duration=1><wave><palette>select one stat upgrade !</+>";

        for (int i = 0; i < 2; i++)
        {
            helperTextTMPA.ResetTime();
            List<StatUpgrade> sUs = GameManager.Instance.GetRandomStatUpgrades(3);

            finishedReward = false;
            yield return _waitForSeconds1;
            for (int j = 0; j < statUpgrades.Count; j++)
            {
                statUpgrades[j].SetUpgrade(sUs[j]);
            }

            while (!finishedReward)
                yield return null;
        }

        helperTextTMP.text = "<+pivot duration=1><wave><palette>select one item !</+>";
        yield return SpawnItemRewardIE();

        helperTextTMP.gameObject.SetActive(false);
        GameManager.Instance.FinishReward();
    }

    IEnumerator SpawnChampionRewardIE()
    {
        championRewardUIs = GameManager.Instance.GetRandomChampionRewardUIs(3);

        finishedReward = false;
        yield return _waitForSeconds1;
        for (int i = 0; i < championRewardUIs.Count; i++)
        {
            championRewardUIs[i].SetAsReward(upgradeZone.transform, championRewardPos[i]);
        }

        while (!finishedReward)
            yield return null;
    }

    IEnumerator SpawnItemRewardIE()
    {
        itemRewards = GameManager.Instance.GetRandomItemRewards(3);

        finishedReward = false;
        yield return _waitForSeconds1;
        for (int i = 0; i < itemRewards.Count; i++)
        {
            itemRewards[i].SetAsReward(upgradeZone.transform, itemRewardPos[i]);
        }

        while (!finishedReward)
            yield return null;
    }

    public void FinishReward() => finishedReward = true;

    /// <summary>
    /// Finish champion reward and schedule destroy rewards that are not selected.
    /// </summary>
    /// <param name="championRewardUI"></param>
    public void FinishChampionReward()
    {
        finishedReward = true;
        championRewardUIs.ForEach(cRUI =>
        {
            cRUI.deactivate();
        });
    }

    /// <summary>
    /// Finish item reward and schedule destroy rewards that are not selected.
    /// </summary>
    /// <param name="item"></param>
    public void FinishItemReward(Item item)
    {
        finishedReward = true;
        itemRewards.ForEach(iR =>
        {
            if (iR != item)
                iR.deactivate();
        });
    }

    public void ShowOnlyStatUpgradeUI(StatUpgradeUI statUpgradeUI)
    {
        for (int i = 0; i < statUpgrades.Count; i++)
        {
            if (statUpgrades[i] != statUpgradeUI)
                statUpgrades[i].gameObject.SetActive(false);
        }
    }

    public void ShowOnlyChampionRewardUI(ChampionRewardUI championRewardUI)
    {
        for (int i = 0; i < championRewardUIs.Count; i++)
        {
            if (championRewardUIs[i] != championRewardUI)
                championRewardUIs[i].gameObject.SetActive(false);
        }
    }

    public void ShowOnlyItemReward(Item item)
    {
        for (int i = 0; i < itemRewards.Count; i++)
        {
            if (itemRewards[i] != item)
                itemRewards[i].HideReward();
        }
    }

    public void ShowStatUpgradeUIs()
    {
        for (int i = 0; i < statUpgrades.Count; i++)
        {
            statUpgrades[i].gameObject.SetActive(true);
        }
    }

    public void ShowChampionRewardUIs()
    {
        for (int i = 0; i < championRewardUIs.Count; i++)
        {
            championRewardUIs[i].gameObject.SetActive(true);
        }
    }

    public void ShowItemRewardUIs()
    {
        for (int i = 0; i < itemRewards.Count; i++)
        {
            itemRewards[i].ShowReward();
        }
    }

    public void TurnOnChampionRewardSelectZone() => championRewardSelectZone.SetActive(true);

    public void TurnOffChampionRewardSelectZone() => championRewardSelectZone.SetActive(false);

    public void TurnOnBuyZone() => buyZone.SetActive(true);

    public void TurnOffBuyZone() => buyZone.SetActive(false);

    public void ApplyScreenEffect(AnimatorController animatorController) =>
        screenEffectAnimator.runtimeAnimatorController = animatorController;

    public void DisableScreenEffect() => screenEffectAnimator.runtimeAnimatorController = null;

    public void HandleTraderUI(List<ChampionRewardUI> championRewardUIs, List<Item> items)
    {
        traderZone.SetActive(true);

        for (int i = 0; i < championWares.Count; i++)
        {
            championWares[i].SetChampionWare(championRewardUIs[i]);
            itemWares[i].SetItemWare(items[i]);
        }
    }

    public void CloseTraderUI()
    {
        traderZone.SetActive(false);
        for (int i = 0; i < championWares.Count; i++)
        {
            championWares[i].ScheduleClearWare();
            itemWares[i].ScheduleClearWare();
        }
    }

    public void UpdatePlayerGold(int gold) => playerGold.SetGold(gold);

    public void SpawnGoldFromDeadEnemies(int gold, Vector3 pos, BasicUI goldUI)
    {
        goldUI.image.sprite = GameManager.Instance.GetRandomGoldSprite();
        goldUI.transform.SetParent(freeZone.transform, false);
        goldUI.transform.position = pos;
        goldUI.LocalMoveTo(
            freeZone.transform.InverseTransformPoint(playerGold.textMeshProUGUI.transform.position),
            2,
            DG.Tweening.Ease.InBack,
            () => GameManager.Instance.UpdateGold(gold)
        );
    }

    public void ShowGuideDialogBox()
    {
        guideDialogBox.Show();
    }

    public void ShowMysteryEvent(MysteryEventDataSO mysteryEventDataSO)
    {
        choicesShown = false;
        eventZone.gameObject.SetActive(true);
        eventZone.tweener.ResetTime();
        eventImage.tweener.ResetTime();
        mysteryEventDescription.tMPWriter.ResetWriter();
        eventChoices.ForEach(c => c.ResetChoice());
        for (int i = 0; i < mysteryEventDataSO.choices.Count; i++)
        {
            eventChoices[i].SetupChoice(i, mysteryEventDataSO);
        }
    }

    public void CloseMysteryEvent()
    {
        eventZone.gameObject.SetActive(false);
    }

    public void HideAllEventChoices() => eventChoices.ForEach(c => c.Hide());

    void WriteEventDescription()
    {
        mysteryEventDescription.tMPWriter.StartWriter();
    }

    bool choicesShown = false;

    void ShowEventChoices()
    {
        if (!choicesShown)
        {
            foreach (var eC in eventChoices)
            {
                if (eC.isAvailable)
                    eC.ShowButton();
            }
            choicesShown = true;
        }
    }

    public void ShowRelic(Relic relic)
    {
        relic.transform.SetParent(relicContent.transform, false);
    }

    public void TakeDamageFromEvent(Action finishCallback)
    {
        eventContainer.transform.DOShakePosition(1, 10).OnComplete(() => finishCallback?.Invoke());
        eventEffect.color = GameManager.Instance.transparentRed;
        eventEffect.DOColor(Vector4.zero, 1);
    }

    public void RewardRelicFromEvent(Relic relic, Action finishCallback)
    {
        relic.transform.SetParent(eventZone.transform, false);
        relic.transform.localPosition = Vector3.zero;
        RectTransform relicRT = relic.transform as RectTransform;
        relicRT
            .DOSizeDelta(new Vector2(300, 300), 1)
            .SetEase(Ease.OutQuint)
            .OnComplete(() =>
            {
                relicRT.transform.DOMove(relicContent.transform.position, 1).SetEase(Ease.InQuint);
                relicRT
                    .DOSizeDelta(new Vector2(50, 50), 1)
                    .SetEase(Ease.OutQuint)
                    .OnComplete(() =>
                    {
                        relic.transform.SetParent(relicContent.transform, false);
                        finishCallback?.Invoke();
                    });
            });
    }

    public GameObject GetNewDoubleTapTooltip()
    {
        GameObject doubleTapTooltip = Instantiate(doubleTapTooltipPrefab);
        doubleTapTooltip.transform.SetParent(freeZone.transform, false);
        return doubleTapTooltip;
    }

    public void LoadScene(SceneMode sceneMode)
    {
        switch (sceneMode)
        {
            case SceneMode.MainMenu:
                SceneManager.LoadSceneAsync("MainMenu");
                break;
            case SceneMode.MainGame:
                SceneManager.LoadSceneAsync("MainGame");
                break;
            default:
                break;
        }
    }

    void ProgressStory(PointerEventData pointerEventData)
    {
        if (storyText.IsWriting)
        {
            storyText.SkipWriter();
        }
        else
        {
            if (currentStoryPage.nextPage == null)
                CloseStory(pointerEventData);
            else
            {
                currentStoryPage = currentStoryPage.nextPage;
                storyImage.sprite = currentStoryPage.sprite;
                storyText.TextComponent.text = currentStoryPage.text;
                storyText.RestartWriter();
            }
        }
    }

    void CloseStory(PointerEventData pointerEventData)
    {
        story.gameObject.SetActive(false);
    }

    public void FinishRestSiteHealOne()
    {
        GameManager.Instance.DisableCampfireInteraction();
        CloseAllRestSiteHealSelectors(null);
    }

    public void ShowCampfireDialogBox()
    {
        campfireDialogBox.Show();
        CloseAllRestSiteHealSelectors(null);
    }

    void ShowAllRestSiteHealSelectors(PointerEventData pointerEventData)
    {
        campfireDialogBox.Hide();
        var playerUnits = GameManager.Instance.GetPlayerTeamChampions();
        restSiteHealSelectors = new();
        foreach (var unit in playerUnits)
        {
            var selector = restSiteHealSelectorPool.PickOne().NPC as RestSiteHealSelector;
            selector.Setup(unit);
            restSiteHealSelectors.Add(selector);
        }
    }

    void CloseAllRestSiteHealSelectors(PointerEventData pointerEventData)
    {
        restSiteHealSelectors.ForEach(s => s.deactivate());
        restSiteHealSelectors.Clear();
    }

    void SelectRestSiteHealAll(PointerEventData pointerEventData)
    {
        GameManager.Instance.HealAllPlayerAlliesByPercentage(0.25f);
        campfireDialogBox.Hide();
        GameManager.Instance.DisableCampfireInteraction();
    }
}
