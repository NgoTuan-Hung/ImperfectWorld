using System;

public class MonoSelfAware : MonoEditor
{
    public Action deactivate;

    public virtual void Awake()
    {
        deactivate = Deactivate;
    }

    public override void Start()
    {
        base.Start();
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
