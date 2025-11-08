using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoEditorSingleton<GameUIManager>
{
    Canvas canvas;
    public Button menuCharButton;
    public GameObject joystickZone,
        joystickPrefab,
        healthAndManaIndicatorPrefab,
        worldSpaceCanvas,
        mapBackground,
        champUIZone,
        inventory,
        inventoryContent,
        freeZone;
    ObjectPool healthAndManaIndicator,
        textPopupUIPool;
    public MapViewUI mapViewUI;
    public InteractiveButtonUI startBattleButton;
    GameObject champInfoPanel;
    Dictionary<CustomMono, ChampInfoPanel> champInfoPanelDict = new();
    Vector3 cameraMoveVector;
    public float cameraMovementSpeed = 0.1f;
    GameObject cameraFollowObject;
    public float planeDistance = 5f;
    public List<GameObject> inventorySlots = new();
    public List<ItemUI> playerItemUIs = new();
    public Vector2 ItemInventoryAnchorPos = new(0.5f, 0.5f);

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuCharButton.onClick.AddListener(() =>
        {
            print("Menu Char Button Clicked");
        });

        InitPrefabAndPool();
        Init();
        FindChilds();
    }

    private void FindChilds()
    {
        mapBackground = transform.Find("MapBackground").gameObject;
        inventoryContent = inventory.transform.Find("Viewport/Content").gameObject;

        /* Temp */
        playerItemUIs = inventoryContent.transform.GetComponentsInChildren<ItemUI>().ToList();
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

    public void RemoveFromInventory(ItemUI itemUI)
    {
        itemUI.transform.parent.SetAsLastSibling();
        inventorySlots.Remove(itemUI.transform.parent.gameObject);
        inventorySlots.Add(itemUI.transform.parent.gameObject);
        itemUI.transform.SetParent(freeZone.transform);
        playerItemUIs.Remove(itemUI);
    }

    public void AddToInventory(ItemUI itemUI)
    {
        itemUI.transform.SetParent(inventorySlots[playerItemUIs.Count].transform);
        itemUI.rectTransform.anchorMin = ItemInventoryAnchorPos;
        itemUI.rectTransform.anchorMax = ItemInventoryAnchorPos;
        itemUI.rectTransform.anchoredPosition = Vector2.zero;
        playerItemUIs.Add(itemUI);
    }
}
