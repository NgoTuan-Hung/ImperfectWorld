using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPointerZone : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
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

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameManager.Instance.positioningPhase)
            return;

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
            }
        }
        else
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

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.Instance.gridManager.HideVisual();
        currentRaycastCM = null;
        previousRaycastNode?.SwitchToDefaultVisual();
        previousRaycastNode = null;
    }

    public void OnBeginDrag(PointerEventData eventData) { }
}
