using UnityEngine;

public class DoActionParamInfo
{
    public Vector2 originToTargetOriginDirection;
    public Vector2 centerToTargetCenterDirection;
    public Vector2 firePointToTargetCenterDirection;
    public Vector2 targetOriginPosition;
    public Vector2 targetCenterPosition;
    public CustomMono target;
    public bool isDirectionModify;
    public float directionModifier;
    public float nextActionChoosingIntervalProposal;
}
