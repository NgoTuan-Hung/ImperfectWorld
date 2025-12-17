using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPointerZone
    : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler,
        IEndDragHandler,
        IBeginDragHandler
{
    CustomMono currentRaycastCM;
    LayerMask layerMask;
    RaycastHit2D raycastHit2D;
    HexGridNode raycastNode = null,
        previousRaycastNode = null;

    private void Awake()
    {
        layerMask = 1 << LayerMask.NameToLayer("CombatCollidee");
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
                currentRaycastCM = GameManager.Instance.GetCustomMono(raycastHit2D.collider);

                GameManager.Instance.gridManager.ShowVisual();
                if (currentRaycastCM != null)
                {
                    GameUIManager.Instance.ShowChampUI(currentRaycastCM);
                    if (!currentRaycastCM.CompareTag("Team1"))
                        currentRaycastCM = null;
                }
            }
        }
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
            if (raycastNode.type != HexGridNodeType.Obstacle && !raycastNode.isEnemyPosition)
            {
                if (raycastNode != previousRaycastNode)
                {
                    previousRaycastNode?.SwitchToDefaultVisual();
                    raycastNode.SwitchToHighlightedVisual();
                    currentRaycastCM.transform.position = raycastNode.pos;
                    previousRaycastNode = raycastNode;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.gridManager.HideVisual();
        currentRaycastCM = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameUIManager.Instance.DeactivateAllChampInfoPanelGlassEffect();
        previousRaycastNode?.SwitchToDefaultVisual();
        previousRaycastNode = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameUIManager.Instance.ActivateAllChampInfoPanelGlassEffect();
    }
}
