using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "GameEffectSO", order = 0)]
public class GameEffectSO : ScriptableObject
{
    public AnimatorController animatorController;
    public bool isDeactivateAfterTime = false;
    public float deactivateTime = 1f;
    public bool isTimeline = false;
    public bool randomRotation = false;
    public float flyAtSpeed = 0.03f;
    public Vector3 followOffset = new(-0.8f, 1.5f, 0);
    public float followSlowlyPositionLerpTime = 0.04f;
    public bool playSound = false;
    public Vector3 effectLocalPosition,
        effectLocalRotation;
    public bool isColoredOverLifetime = false;
    public Gradient colorOverLifetimeGrad;
    public TimelineAsset timelineAsset;
    public bool useBoxCollider = false;
    public bool usePolygonCollider = false;
    public bool useCircleCollider = false;
    public BoxCollider2D boxCollider2D;
    public PolygonCollider2D polygonCollider2D;
    public CircleCollider2D circleCollider2D;
    public EGameEffectBehaviour useBehaviour = EGameEffectBehaviour.None;
    public CollideAndDamageSO collideAndDamageSO;
    public AudioSource audioSource;

    #region SpriteRenderer
    [Header("Sprite Renderer")]
    public GameEffectPrefab gameEffectPrefab;
    #endregion
    public bool useSecondarySpriteRenderer = false;
    public bool useTrailRenderer = false;
}
