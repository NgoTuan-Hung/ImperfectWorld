using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "GameEffectSO", order = 0)]
public class GameEffectSO : ScriptableObject
{
    public Animator animator;
    public float deactivateTime = 1f;
    public bool isDeactivateAfterTime = false;
    public bool isTimeline = false;
    public bool playSound = false;
    public bool randomRotation = false;
    public bool isColoredOverLifetime = false;
    public float flyAtSpeed = 0.03f;
    public Vector3 followOffset = new(-0.8f, 1.5f, 0);
    public float followSlowlyPositionLerpTime = 0.04f;
    public Gradient colorOverLifetimeGrad;
    public TimelineAsset timelineAsset;
    public EGameEffectBehaviour behaviour;
    public bool useBoxCollider = false;
    public bool usePolygonCollider = false;
    public BoxCollider2D boxCollider2D;
    public PolygonCollider2D polygonCollider2D;
    public EGameEffectBehaviour useBehaviour = EGameEffectBehaviour.None;
    public CollideAndDamageSO collideAndDamageSO;
}
