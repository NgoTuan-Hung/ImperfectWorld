using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    List<PoolObject> pool = new();

    /// <summary>
    /// This is what we call when initializing any poolObject so that we could
    /// use it components with ease everytime we have a poolObject. Like
    /// poolObject.gameEffect.DoAnythingWeWant()
    /// </summary>
    public Action<PoolObject> handleCachedComponentRefs = (poolObject) => { };
    GameObject prefab;
    static readonly float destroyAfter = 15f;

    /// <summary>
    /// Initializing an object pool of with predefined prefab. Component properties
    /// are cached before hand so next time we can access them without having to called
    /// GetComponent or GetComponentInChildren which are expensive.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="size"></param>
    /// <param name="poolArguments"></param>
    public ObjectPool(GameObject prefab, params PoolArgument[] poolArguments)
    {
        this.prefab = prefab;

        /* Predetermine component caching rule before hand by creating an action base on the criteria provided instead
        of having to do big calculation for every pool object. For example:
        forewach (poolObject in pool)
        {
            if (poolArgument.whereComponent == PoolArgument.WhereComponent.Self) ...
        }
        
        so instead of that we could do forewach (poolObject in pool) { handleCachedComponentRefs(poolObject); }
        */
        foreach (PoolArgument poolArgument in poolArguments)
        {
            Func<Type, PoolObject, Component> getComponentInLocation;

            /* Decide whether to get component in self or children. */
            if (poolArgument.whereComponent == PoolArgument.WhereComponent.Self)
            {
                getComponentInLocation = GetComponent;
            }
            else
                getComponentInLocation = GetComponentInChildren; //

            /* Set corresponding cached property based on what component we want to get. */
            switch (poolArgument.componentType)
            {
                case ComponentType.HealthAndManaIndicator:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.HealthAndManaIndicator,
                            getComponentInLocation(typeof(HealthAndManaIndicator), poolObject)
                        );
                    };
                    break;
                case ComponentType.GameEffect:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.GameEffect,
                            getComponentInLocation(typeof(GameEffect), poolObject)
                        );

                        poolObject.GameEffect.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.WorldSpaceUI:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.WorldSpaceUI,
                            getComponentInLocation(typeof(WorldSpaceUI), poolObject)
                        );

                        poolObject.WorldSpaceUI.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.CustomMono:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.CustomMono,
                            getComponentInLocation(typeof(CustomMono), poolObject)
                        );

                        poolObject.CustomMono.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.TextPopupUI:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.TextPopupUI,
                            getComponentInLocation(typeof(TextPopupUI), poolObject)
                        );

                        poolObject.TextPopupUI.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.Item:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.Item,
                            getComponentInLocation(typeof(Item), poolObject)
                        );

                        poolObject.Item.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.ChampionRewardUI:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.ChampionRewardUI,
                            getComponentInLocation(typeof(ChampionRewardUI), poolObject)
                        );

                        poolObject.ChampionRewardUI.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.Relic:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.Relic,
                            getComponentInLocation(typeof(Relic), poolObject)
                        );

                        poolObject.Relic.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.BasicUI:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.BasicUI,
                            getComponentInLocation(typeof(BasicUI), poolObject)
                        );

                        poolObject.BasicUI.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.NPC:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.AddComponent(
                            ComponentType.NPC,
                            getComponentInLocation(typeof(NPC), poolObject)
                        );

                        poolObject.NPC.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                default:
                    break;
            }
        }
    }

    public PoolObject PickOne()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                if (pool[i].idleScheme != null)
                    GameManager.Instance.StopCoroutine(pool[i].idleScheme);
                return pool[i];
            }
        }

        PoolObject t_poolObject = new() { gameObject = GameObject.Instantiate(prefab) };
        handleCachedComponentRefs(t_poolObject);
        pool.Add(t_poolObject);

        return t_poolObject;
    }

    /// <summary>
    /// Destroy objects that are not used after a while.
    /// </summary>
    void IdleScheme(PoolObject p_poolObject)
    {
        GameManager.Instance.StartCoroutine(p_poolObject.idleScheme = IdleSchemeIE(p_poolObject));
    }

    IEnumerator IdleSchemeIE(PoolObject p_poolObject)
    {
        yield return new WaitForSeconds(destroyAfter);

        pool.Remove(p_poolObject);
        GameObject.Destroy(p_poolObject.gameObject);
    }

    Component GetComponent(Type type, PoolObject poolObject) =>
        poolObject.gameObject.GetComponent(type);

    Component GetComponentInChildren(Type type, PoolObject poolObject) =>
        poolObject.gameObject.GetComponentInChildren(type);

    public GameEffect PickOneGameEffect() => PickOne().GameEffect;
}
