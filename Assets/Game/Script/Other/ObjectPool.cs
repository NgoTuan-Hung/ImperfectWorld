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
                        poolObject.healthAndManaIndicator =
                            getComponentInLocation(typeof(HealthAndManaIndicator), poolObject)
                            as HealthAndManaIndicator;
                    };
                    break;
                case ComponentType.GameEffect:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.gameEffect =
                            getComponentInLocation(typeof(GameEffect), poolObject) as GameEffect;

                        poolObject.gameEffect.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.WorldSpaceUI:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.worldSpaceUI =
                            getComponentInLocation(typeof(WorldSpaceUI), poolObject)
                            as WorldSpaceUI;

                        poolObject.worldSpaceUI.deactivate += () => IdleScheme(poolObject);
                    };
                    break;
                case ComponentType.CustomMono:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.customMono =
                            getComponentInLocation(typeof(CustomMono), poolObject) as CustomMono;

                        poolObject.customMono.deactivate += () => IdleScheme(poolObject);
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

    public PoolObject PickOne(Action<PoolObject> initAction)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                initAction(pool[i]);
                pool[i].gameObject.SetActive(true);
                if (pool[i].idleScheme != null)
                    GameManager.Instance.StopCoroutine(pool[i].idleScheme);
                return pool[i];
            }
        }

        PoolObject t_poolObject = new() { gameObject = GameObject.Instantiate(prefab) };
        handleCachedComponentRefs(t_poolObject);
        initAction(t_poolObject);
        pool.Add(t_poolObject);

        return t_poolObject;
    }

    public List<PoolObject> Pick(int n)
    {
        int count = 0;
        List<PoolObject> poolObjects = new List<PoolObject>();
        for (int i = 0; i < pool.Count; i++)
        {
            if (count >= n)
                return poolObjects;

            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                if (pool[i].idleScheme != null)
                    GameManager.Instance.StopCoroutine(pool[i].idleScheme);
                poolObjects.Add(pool[i]);
                count++;
            }
        }

        /* In case the list is not enough */
        while (count < n)
        {
            PoolObject t_poolObject = new() { gameObject = GameObject.Instantiate(prefab) };
            handleCachedComponentRefs(t_poolObject);
            pool.Add(t_poolObject);
            poolObjects.Add(t_poolObject);
            count++;
        }

        return poolObjects;
    }

    public List<PoolObject> PickAndPlace(int n, Vector3 position)
    {
        int count = 0;
        List<PoolObject> poolObjects = new List<PoolObject>();
        for (int i = 0; i < pool.Count; i++)
        {
            if (count >= n)
                return poolObjects;

            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                pool[i].gameObject.transform.position = position;
                if (pool[i].idleScheme != null)
                    GameManager.Instance.StopCoroutine(pool[i].idleScheme);
                poolObjects.Add(pool[i]);
                count++;
            }
        }

        /* In case the list is not enough */
        while (count < n)
        {
            PoolObject t_poolObject = new() { gameObject = GameObject.Instantiate(prefab) };
            handleCachedComponentRefs(t_poolObject);
            t_poolObject.gameObject.transform.position = position;
            pool.Add(t_poolObject);
            poolObjects.Add(t_poolObject);
            count++;
        }

        return poolObjects;
    }

    Component GetComponent(Type type, PoolObject poolObject) =>
        poolObject.gameObject.GetComponent(type);

    Component GetComponentInChildren(Type type, PoolObject poolObject) =>
        poolObject.gameObject.GetComponentInChildren(type);

    public void ForEach(Action<PoolObject> action)
    {
        for (int i = 0; i < pool.Count; i++)
            action(pool[i]);
    }
}
