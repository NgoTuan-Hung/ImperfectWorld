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

#if false
    PointerDown(eventData){
        raycastHit2D = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(eventData.position),
                Vector3.forward,
                10,
                layerMask
            );
        
            if (raycastHit2D.collider != null)
            {
                currentRaycastCM = GameManager.Instance.GetCustomMono(raycastHit2D.collider);

                GameUIManager.Instance.ShowChampUI(currentRaycastCM);
            }
    }
#endif

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
                GameUIManager.Instance.ShowChampUI(currentRaycastCM);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameManager.Instance.positioningPhase)
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
        previousRaycastNode?.SwitchToDefaultVisual();
        previousRaycastNode = null;
    }

    public void OnBeginDrag(PointerEventData eventData) { }
}
