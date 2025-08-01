using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "GameEffectSO", order = 0)]
public class GameEffectSO : ScriptableObject
{
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
    public CollideAndDamageSO collideAndDamageSO;
    public string tag = "Untagged";

    // default exclude everything except CombatCollidee
    public LayerMask collisionExcludeLayerMask;
    public List<EGameEffectBehaviour> gameEffectBehaviours;
    public bool useParticleSystem = false;
}
