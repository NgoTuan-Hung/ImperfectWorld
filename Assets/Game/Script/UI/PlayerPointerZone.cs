using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPointerZone
    : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler,
        IBeginDragHandler
{
    CustomMono currentRaycastCM;
    LayerMask layerMask;
    RaycastHit2D raycastHit2D;
    HexGridNode raycastNode = null,
        previousRaycastNode = null;
    int combatCollideeLayer,
        nPCLayer;

    private void Awake()
    {
        combatCollideeLayer = LayerMask.NameToLayer("CombatCollidee");
        nPCLayer = LayerMask.NameToLayer("NPC");
        layerMask = (1 << combatCollideeLayer) | (1 << nPCLayer);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentRaycastCM == null)
        {
            raycastHit2D = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(eventData.position),
                Vector3.forward,
                10,
                layerMask
            );
            if (raycastHit2D.collider != null)
            {
                if (raycastHit2D.collider.gameObject.layer == combatCollideeLayer)
                {
                    currentRaycastCM = GameManager.Instance.GetCustomMono(raycastHit2D.collider);

                    if (currentRaycastCM != null)
                    {
                        GameUIManager.Instance.ShowChampUI(currentRaycastCM);
                        if (!currentRaycastCM.CompareTag("Team1"))
                            currentRaycastCM = null;
                    }
                }
                else if (raycastHit2D.collider.gameObject.layer == nPCLayer)
                {
                    var npc = GameManager.Instance.GetNPC(raycastHit2D.collider);

                    if (npc != null)
                    {
                        npc.interact();
                    }
                }
            }
        }
        GameManager.Instance.gridManager.ShowVisual();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.gameState != GameState.PositioningPhase)
            return;

        if (currentRaycastCM != null)
        {
            raycastNode = GameManager.Instance.gridManager.GetNodeAtPosition(
                Camera.main.ScreenToWorldPoint(eventData.position)
            );
            if (
                raycastNode.type != HexGridNodeType.Obstacle
                && !HexGridManager.Instance.IsOccupied(raycastNode)
            )
            {
                previousRaycastNode = HexGridManager.Instance.GetOccupyNode(currentRaycastCM);
                if (raycastNode != previousRaycastNode)
                {
                    HexGridManager.Instance.RemoveOccupy(previousRaycastNode);
                    HexGridManager.Instance.SetOccupiedNodeHighlight(currentRaycastCM, raycastNode);
                    currentRaycastCM.transform.position = raycastNode.pos;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.gridManager.HideVisual();
        GameUIManager.Instance.DeactivateAllChampInfoPanelGlassEffect();
        if (
            GameManager.Instance.gameState == GameState.PositioningPhase
            && currentRaycastCM != null
        )
        {
            HexGridManager.Instance.GetOccupyNode(currentRaycastCM).SwitchToOccupiedVisual();
        }
        currentRaycastCM = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameUIManager.Instance.ActivateAllChampInfoPanelGlassEffect();
    }
}
