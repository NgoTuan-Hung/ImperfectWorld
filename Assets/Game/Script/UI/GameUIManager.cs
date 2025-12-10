using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using TMPEffects.Components;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoEditorSingleton<GameUIManager>
{
    private static WaitForSeconds _waitForSeconds1 = new(1f);
    Canvas canvas;
    public Button menuCharButton,
        menuMapButton,
        menuInventoryButton,
        menuSettingButton;
    TextMeshProUGUI helperTextTMP;
    public GameObject joystickZone,
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
        championWare;
    ObjectPool healthAndManaIndicator,
        textPopupUIPool;
    public MapViewUI mapViewUI;
    public InteractiveButtonUI startBattleButton;
    GameObject champInfoPanel;
    Dictionary<CustomMono, ChampInfoPanel> champInfoPanelDict = new();
    Vector3 cameraMoveVector;
    public float cameraMovementSpeed = 0.1f;
    public GameObject cameraFollowObject;
    public float planeDistance = 5f;
    public List<GameObject> inventorySlots = new();
    public List<Item> playerItemUIs = new();
    public Vector2 ItemInventoryAnchorPos = new(0.5f, 0.5f);
    bool mapState = true;
    public List<StatUpgradeUI> statUpgrades;
    public List<ChampionRewardUI> championRewardUIs;
    bool finishedReward = false;
    bool menuCharStateOn = false;
    bool menuItemStateOn = false;
    public Image screenEffectImg;
    public Animator screenEffectAnimator;
    TMPAnimator helperTextTMPA;
    public Vector2[] itemRewardPos = new Vector2[3];
    public Vector2[] championRewardPos = new Vector2[3];
    List<Item> itemRewards;
    List<TraderWare> championWares;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        InitPrefabAndPool();
        FindChilds();
        Init();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        menuCharButton.onClick.AddListener(ClickMenuCharButton);
        menuMapButton.onClick.AddListener(ClickMenuMapButton);
        menuInventoryButton.onClick.AddListener(ClickMenuItemButton);
    }

    void ClickMenuCharButton()
    {
        if (menuCharStateOn)
        {
            foreach (var item in champInfoPanelDict)
            {
                item.Value.OnTabClick(item.Value.xIPTB);
            }
            menuCharStateOn = false;
        }
        else
        {
            foreach (var item in champInfoPanelDict)
            {
                item.Value.ShowIfAlive();
                item.Value.OnTabClick(item.Value.statIPTB);
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
                ShowChampUI(item.Value);
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
        mapBackground = transform.Find("MapBackground").gameObject;
        inventoryContent = inventory.transform.Find("Viewport/Content").gameObject;
        menu = transform.Find("Menu").gameObject;
        menuCharButton = menu.transform.Find("MenuCharButton").GetComponent<Button>();
        menuMapButton = menu.transform.Find("MenuMapButton").GetComponent<Button>();
        menuInventoryButton = menu.transform.Find("MenuInventoryButton").GetComponent<Button>();
        menuSettingButton = menu.transform.Find("MenuSettingButton").GetComponent<Button>();
        upgradeZone = transform.Find("MainScreen/UpgradeZone").gameObject;
        statUpgrades = upgradeZone.transform.GetComponentsInChildren<StatUpgradeUI>(true).ToList();
        helperTextTMP = transform.Find("MainScreen/HelperText").GetComponent<TextMeshProUGUI>();
        helperTextTMPA = helperTextTMP.GetComponent<TMPAnimator>();
        championRewardSelectZone = transform.Find("MainScreen/ChampionRewardSelectZone").gameObject;
        traderZone = worldSpaceCanvas.transform.Find("TraderZone").gameObject;
        championWare = traderZone.transform.Find("ChampionWare").gameObject;
        championWares = championWare.GetComponentsInChildren<TraderWare>(true).ToList();

        /* Temp */
        playerItemUIs = inventoryContent.transform.GetComponentsInChildren<Item>().ToList();
        for (int i = 0; i < inventoryContent.transform.childCount; i++)
        {
            inventorySlots.Add(inventoryContent.transform.GetChild(i).gameObject);
        }
    }

    public override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        cameraFollowObject.transform.position += cameraMoveVector;
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
        mapBackground.SetActive(true);
    }

    public PoolObject CreateAndHandleHPAndMPUIWithFollowing(Transform transform)
    {
        PoolObject healthAndManaIndicatorObj = healthAndManaIndicator.PickOne();
        healthAndManaIndicatorObj.WorldSpaceUI.FollowSlowly(transform);
        return healthAndManaIndicatorObj;
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

    public void ShowChampUI(ChampInfoPanel p_champInfoPanel) =>
        p_champInfoPanel.gameObject.SetActive(true);

    public void RemoveFromInventory(Item itemUI)
    {
        itemUI.transform.parent.SetAsLastSibling();
        inventorySlots.Remove(itemUI.transform.parent.gameObject);
        inventorySlots.Add(itemUI.transform.parent.gameObject);
        itemUI.transform.SetParent(freeZone.transform);
        playerItemUIs.Remove(itemUI);
    }

    public void AddToInventory(Item itemUI)
    {
        itemUI.transform.SetParent(inventorySlots[playerItemUIs.Count].transform);
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

    public void ApplyScreenEffect(AnimatorController animatorController) =>
        screenEffectAnimator.runtimeAnimatorController = animatorController;

    public void DisableScreenEffect() => screenEffectAnimator.runtimeAnimatorController = null;

    public void HandleTraderUI(List<ChampionRewardUI> championRewardUIs)
    {
        traderZone.SetActive(true);

        for (int i = 0; i < championRewardUIs.Count; i++)
        {
            championRewardUIs[i].SetAsShopWare(championWares[i].transform);
        }
    }
}
