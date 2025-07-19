using System.Collections;
using System.Linq;
using UnityEngine;

public class BladeOfVu : SkillBase
{
    SpriteList spriteList;
    GameObject starPos;

    public override void Awake()
    {
        base.Awake();
        maxAmmo = 10;
        cooldown = 5f;
        maxRange = 4f;
        interval = 0.01f;
        starPos = GetComponentsInChildren<Transform>()
            .First(x => x.name.Equals("StarPos"))
            .gameObject;
        damage = defaultDamage = 10f;
        /* Get sprite list from res */
        spriteList = Resources.Load<SpriteList>("BladeOfVuSpriteList");
        successResult = new(true, ActionResultType.Cooldown, cooldown);
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
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking && !customMono.movementActionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
            StartCoroutine(actionIE = TriggerIE(location, direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerIE(Vector2 location = default, Vector2 direction = default)
    {
        GameEffect t_star = GameManager
            .Instance.gameEffectPool.PickOne()
            .gameEffect.Init(GameManager.Instance.bladeOfVuStarSO);
        t_star.transform.position = customMono.spriteRenderer.transform.TransformPoint(
            starPos.transform.localPosition
        );
        yield return new WaitForSeconds(0.33f);

        /* Handle dissapearance */

        customMono.boxCollider2D.enabled = false;
        customMono.combatCollider2D.enabled = false;
        customMono.spriteRenderer.enabled = false;

        currentAmmo = 0;
        while (currentAmmo < maxAmmo)
        {
            GameEffect t_ghost = GameManager
                .Instance.gameEffectPool.PickOne()
                .gameEffect.Init(GameManager.Instance.ghostSO);
            t_ghost.animateObjects[0].spriteRenderer.sprite = spriteList.sprites[
                Random.Range(0, spriteList.sprites.Count)
            ];

            t_ghost.transform.position =
                transform.position
                + new Vector3(Random.Range(-1, 1f), Random.Range(-0.5f, 1f), 0).normalized
                    * Random.Range(0, maxRange);
            t_ghost.animateObjects[0].transform.localScale = transform.localScale.WithX(
                Random.Range(0, 2) == 0 ? -1 : 1
            );

            yield return new WaitForSeconds(interval);
            currentAmmo++;
        }

        /* Spawn slash */
        CollideAndDamage t_slash = (CollideAndDamage)
            GameManager
                .Instance.gameEffectPool.PickOne()
                .gameEffect.Init(GameManager.Instance.bladeOfVuSlashSO)
                .GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
        t_slash.allyTags = customMono.allyTags;
        t_slash.collideDamage = damage;
        t_slash.transform.position = transform.position;

        /* Handle reappearance */

        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        customMono.spriteRenderer.enabled = true;
        customMono.boxCollider2D.enabled = true;
        customMono.combatCollider2D.enabled = true;

        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.currentAction = null;
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
        StopCoroutine(actionIE);
        customMono.currentAction = null;
    }
}
