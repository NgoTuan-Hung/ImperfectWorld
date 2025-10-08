using System.Collections;
using UnityEngine;

public class Movable : BaseAction
{
    public Vector2 moveVector;

    /// <summary>
    /// Should be used to toggle between player controlling and AI controlling
    /// </summary>
    public PausableScript pausableScript = new();

    public override void Awake()
    {
        base.Awake();
        AddActionManuals();

        pausableScript.resumeFixedUpdate = () => pausableScript.fixedUpdate = MoveByController;
        pausableScript.pauseFixedUpdate = () => pausableScript.fixedUpdate = EmptyFixedUpdate;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
    }

    public override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        pausableScript.fixedUpdate();
    }

    void MoveByController()
    {
        if (moveVector != Vector2.zero)
            Move(moveVector);
        else
            ToggleMoveAnim(false);
    }

    public void Move(Vector2 direction)
    {
        if (canUse && !customMono.movementActionBlocking)
        {
            if (!GetMoveBool())
                ToggleMoveAnim(true);
            customMono.SetUpdateDirectionIndicator(
                direction,
                UpdateDirectionIndicatorPriority.VeryLow
            );
            transform.position += (Vector3)direction.normalized * customMono.stat.moveSpeedPerFrame;
        }
    }

    public void StopMove() => ToggleMoveAnim(false);

    public void ToggleMoveAnim(bool value)
    {
        customMono.AnimatorWrapper.animator.SetBool(GameManager.Instance.walkBoolHash, value);
    }

    public bool GetMoveBool() => GetBool(GameManager.Instance.walkBoolHash);

    void EmptyFixedUpdate() { }
}
