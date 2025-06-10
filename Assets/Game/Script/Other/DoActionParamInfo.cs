using UnityEngine;

public class DoActionParamInfo
{
    public Vector2 originToTargetOriginDirection;
    public Vector2 centerToTargetCenterDirection;
    public Vector2 firePointToTargetCenterDirection;
    public Vector2 targetOriginPosition;
    public Vector2 targetCenterPosition;
    public bool isDirectionModify;
    public float directionModifier;
    public float nextActionChoosingIntervalProposal;

    public DoActionParamInfo(
        Vector2 originToTargetOriginDirection = default,
        Vector2 centerToTargetCenterDirection = default,
        Vector2 firePointToTargetCenterDirection = default,
        Vector2 targetOriginPosition = default,
        Vector2 targetCenterPosition = default,
        bool isDirectionModify = false,
        float directionModifier = 0,
        float nextActionChoosingIntervalProposal = 0
    )
    {
        this.originToTargetOriginDirection = originToTargetOriginDirection;
        this.centerToTargetCenterDirection = centerToTargetCenterDirection;
        this.firePointToTargetCenterDirection = firePointToTargetCenterDirection;
        this.targetOriginPosition = targetOriginPosition;
        this.targetCenterPosition = targetCenterPosition;
        this.isDirectionModify = isDirectionModify;
        this.directionModifier = directionModifier;
        this.nextActionChoosingIntervalProposal = nextActionChoosingIntervalProposal;
    }
}
