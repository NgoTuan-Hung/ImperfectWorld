using UnityEngine;

public class ActionRequiredParameter
{
    public Vector2 originToTargetOriginDirection;
    public Vector2 centerToTargetCenterDirection;
    public Vector2 targetOriginPosition;
    public Vector2 targetCenterPosition;

    public void Refresh()
    {
        originToTargetOriginDirection = Vector2.zero;
        centerToTargetCenterDirection = Vector2.zero;
        targetOriginPosition = Vector3.zero;
        targetCenterPosition = Vector3.zero;
    }
}
