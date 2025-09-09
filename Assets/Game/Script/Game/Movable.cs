using System.Collections;
using UnityEngine;

public class Movable : BaseAction
{
    public Vector2 moveVector;
    public PausableScript pausableScript = new();

    public override void Awake()
    {
        base.Awake();
        AddActionManuals();

        pausableScript.resumeFixedUpdate = () => pausableScript.fixedUpdate = MoveByController;
        pausableScript.pauseFixedUpdate = () => pausableScript.fixedUpdate = () => { };
        pausableScript.pauseFixedUpdate();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.GetCloser,
                (p_doActionParamInfo) =>
                    MoveTo(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.GetAway,
                (p_doActionParamInfo) =>
                    MoveTo(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(
                    nextActionChoosingIntervalProposal: 0.5f,
                    isDirectionModify: true,
                    directionModifier: -1
                )
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.Dodge,
                (p_doActionParamInfo) =>
                    MoveTo(
                        new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.Passive,
                (p_doActionParamInfo) =>
                    Idle(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.Roam,
                (p_doActionParamInfo) =>
                    MoveTo(
                        new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 1f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.Roam,
                (p_doActionParamInfo) =>
                    Idle(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 1f)
            )
        );
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

    public void ToggleMoveAnim(bool value)
    {
        customMono.AnimatorWrapper.animator.SetBool(GameManager.Instance.walkBoolHash, value);
    }

    public bool GetMoveBool() => GetBool(GameManager.Instance.walkBoolHash);

    public void MoveTo(Vector2 direction, float duration)
    {
        StartCoroutine(MoveToCoroutine(direction, duration));
    }

    IEnumerator MoveToCoroutine(Vector2 direction, float duration)
    {
        float currentTime = 0;
        customMono.movementActionInterval = true;
        while (currentTime < duration)
        {
            customMono.movable.Move(direction);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
        customMono.movementActionInterval = false;
    }

    public void Idle(Vector2 direction, float duration)
    {
        StartCoroutine(IdleCoroutine(direction, duration));
    }

    IEnumerator IdleCoroutine(Vector2 direction, float duration)
    {
        customMono.movementActionInterval = true;
        customMono.movable.ToggleMoveAnim(false);
        yield return new WaitForSeconds(duration);

        customMono.movementActionInterval = false;
    }
}
