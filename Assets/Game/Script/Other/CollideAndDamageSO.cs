using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "CollideAndDamageSO", order = 0)]
public class CollideAndDamageSO : ScriptableObject
{
    public enum CollideType
    {
        Single,
        Multiple,
    }

    public CollideType collideType = CollideType.Single;
    public OneTimeContactInteraction oneTimeContactInteraction = OneTimeContactInteraction.None;

    public enum PushEnemyOnCollideType
    {
        Random,
        BothSide,
        LaterDecide,
    };

    public PushEnemyOnCollideType pushEnemyOnCollideType = PushEnemyOnCollideType.Random;
    public bool deactivateOnCollide = false;
    public bool spawnEffectOnCollide = false;
    public GameEffectSO spawnedEffectOnCollide = null;
}
