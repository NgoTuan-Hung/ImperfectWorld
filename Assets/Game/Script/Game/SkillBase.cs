using UnityEngine;

public class SkillBase : BaseAction
{
    public float duration;
    public float maxRange;
    public float interval;
    public Vector3 effectActiveLocation = new Vector3(0, 999, 0);
    public float manaCost;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }
}
