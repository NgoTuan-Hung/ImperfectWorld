using System.Collections;
using System.Linq;
using UnityEngine;

public class BladeOfVu : SkillBase
{
    static SpriteList spriteList;
    static readonly WaitForSeconds waitForSeconds = new(0.33f);

    public override void Awake()
    {
        base.Awake();
        /* Get sprite list from res */
        spriteList =
            spriteList != null ? spriteList : Resources.Load<SpriteList>("BladeOfVuSpriteList");
        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new(
                ActionUse.MeleeDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.45f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        /* Position of the star effect */
        GetActionField<ActionGameObjectField>(ActionFieldName.CustomGameObject).value =
            GetComponentsInChildren<Transform>().First(x => x.name.Equals("StarPos")).gameObject;
        GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 10;
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 4f;
        GetActionField<ActionFloatField>(ActionFieldName.Interval).value = 0.01f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 20f;
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value =
            GetActionField<ActionIntField>(ActionFieldName.EffectCount).value
            * GetActionField<ActionFloatField>(ActionFieldName.Interval).value;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.might.FinalValue * 0.2f;
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse && !customMono.actionBlocking && !customMono.movementActionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
                    location,
                    direction
                )
            );
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerIE(Vector2 location = default, Vector2 direction = default)
    {
        SpawnNormalEffect(
            GameManager.Instance.bladeOfVuStarPool.PickOneGameEffect(),
            GetActionField<ActionGameObjectField>(
                ActionFieldName.CustomGameObject
            ).value.transform.position
        );

        yield return waitForSeconds;

        /* Flash then spawn slash */
        StartCoroutine(SpawnGhost());
        yield return Flash(
            Vector3.zero,
            0,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value
        );
        SpawnNormalEffect(
            GameManager.Instance.bladeOfVuSlashPool.PickOneGameEffect(),
            transform.position,
            p_isCombat: true
        );

        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.currentAction = null;
    }

    IEnumerator SpawnGhost()
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.EffectCount).value; i++)
        {
            GameEffect t_ghost = GameManager.Instance.ghostPool.PickOne().gameEffect;
            t_ghost.animateObjects[0].spriteRenderer.sprite = spriteList.sprites[
                Random.Range(0, spriteList.sprites.Count)
            ];

            t_ghost.transform.position =
                transform.position
                + new Vector3(Random.Range(-1, 1f), Random.Range(-0.5f, 1f), 0).normalized
                    * Random.Range(
                        0,
                        GetActionField<ActionFloatField>(ActionFieldName.Range).value
                    );
            t_ghost.animateObjects[0].transform.localScale = transform.localScale.WithX(
                Random.Range(0, 2) == 0 ? -1 : 1
            );

            yield return new WaitForSeconds(
                GetActionField<ActionFloatField>(ActionFieldName.Interval).value
            );
        }
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_direction, p_duration));
    }

    IEnumerator BotTriggerIE(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
    }
}
