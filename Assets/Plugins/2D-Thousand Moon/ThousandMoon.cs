using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThousandMoon : MonoBehaviour
{
    public GameObject effectPrefab, slashEffect;
    public float cooldown = 4f;
    public float effectDuration = 2f;
    public float totalEffect = 5;
    public bool canUse = true;
    public float areaRadius = 4f;
    public float effectTravelSpeed = 4f;
    public float spawnDelayDefault = 0.5f;
    public float spawnDelay;
    public bool isRandomSpawnDelay = false;
    Action changeSpawnDelay = () => {};
    public bool startSkill = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isRandomSpawnDelay) changeSpawnDelay = () => {spawnDelay = Random.Range(0, spawnDelayDefault);};
        else spawnDelay = spawnDelayDefault;
    }

    // Update is called once per frame
    void Update()
    {
        if (startSkill)
        {
            startSkill = false;
            if (canUse)
            {
                canUse = false;
                StartCoroutine(ResetSkillCooldown());
                StartCoroutine(SpawnEffect());
            }
        }
    }
    
    IEnumerator SpawnEffect()
    {
        for (int i=0;i<totalEffect;i++)
        {
            StartCoroutine(EffectMoveAndFade());
            changeSpawnDelay();
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    IEnumerator EffectMoveAndFade()
    {
        GameObject t_effect = Instantiate(effectPrefab);
        SpriteRenderer t_spriteRenderer = t_effect.GetComponent<SpriteRenderer>();
        
        
        float t_radius = Random.Range(0, areaRadius);
        Vector2 spawnPos = new(); 
        Vector3 t_moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * effectTravelSpeed;
        spawnPos.x = Random.Range(0, t_radius); 
        spawnPos.y = (float)Math.Sqrt(t_radius * t_radius - spawnPos.x * spawnPos.x) * (Random.Range(0, 2) == 0 ? 1 : -1);
        t_effect.transform.position = transform.position + new Vector3(spawnPos.x, spawnPos.y);
        t_effect.transform.localScale = new Vector3(t_moveDir.x > 0 ? 1 : -1, 1, 1);
        
        Stopwatch t_stopWatch = new(); t_stopWatch.Restart();
        
        {
            SamuraiEffect t_samuraiEffect = t_effect.GetComponent<SamuraiEffect>();
            t_samuraiEffect.moveDir = t_moveDir;
            t_samuraiEffect.slashEffect = slashEffect;
            t_samuraiEffect.WaitForSignal();
        }
        
        while (t_stopWatch.Elapsed.TotalSeconds < effectDuration)
        {
            t_effect.transform.position += t_moveDir * Time.deltaTime;
            t_spriteRenderer.color = new Color(t_spriteRenderer.color.r, t_spriteRenderer.color.g, t_spriteRenderer.color.b
            , 1 - (float)t_stopWatch.Elapsed.TotalSeconds / effectDuration);
            
        
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        t_stopWatch.Stop();
        Destroy(t_effect);
    }
    
    IEnumerator ResetSkillCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        
        canUse = true;
    }
}
