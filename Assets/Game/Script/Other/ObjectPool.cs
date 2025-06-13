using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    List<PoolObject> pool;
    Action<PoolObject> handleCachedComponentRefs = (poolObject) => { };
    int intendedSize,
        threeOutOfFour,
        currentActive = 0;
    GameObject prefab;

    /// <summary>
    /// Initializing an object pool of size n with predefined prefab. Component properties
    /// are cached before hand so next time we can access them without having to called
    /// GetComponent or GetComponentInChildren which are expensive.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="size"></param>
    /// <param name="poolArguments"></param>
    public ObjectPool(GameObject prefab, int size, params PoolArgument[] poolArguments)
    {
        intendedSize = size;
        threeOutOfFour = size * 3 / 4;
        this.prefab = prefab;

        /* Predetermine component caching rule before hand by creating an action base on the criteria provided instead
        of having to do big calculation for every pool object. */
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
                case ComponentType.RadialProgress:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.radialProgress =
                            getComponentInLocation(typeof(RadialProgress), poolObject)
                            as RadialProgress;
                    };
                    break;
                case ComponentType.GameEffect:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.gameEffect =
                            getComponentInLocation(typeof(GameEffect), poolObject) as GameEffect;

                        poolObject.gameEffect.deactivate += () => currentActive--;
                    };
                    break;
                case ComponentType.CustomMono:
                    handleCachedComponentRefs += (poolObject) =>
                    {
                        poolObject.customMono =
                            getComponentInLocation(typeof(CustomMono), poolObject) as CustomMono;

                        poolObject.customMono.deactivate += () => currentActive--;
                    };
                    break;
                default:
                    break;
            }
        }

        pool = new List<PoolObject>(size);

        for (int i = 0; i < pool.Capacity; i++)
        {
            PoolObject poolObject = new() { gameObject = GameObject.Instantiate(prefab) };

            handleCachedComponentRefs(poolObject);
            poolObject.gameObject.SetActive(false);

            pool.Add(poolObject);
        }
    }

    public PoolObject PickOne()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                currentActive++;
                GrowChecker();
                return pool[i];
            }
        }

        return null;
    }

    public PoolObject PickOne(Action<PoolObject> initAction)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                initAction(pool[i]);
                pool[i].gameObject.SetActive(true);
                currentActive++;
                GrowChecker();
                return pool[i];
            }
        }

        return null;
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
                currentActive++;
                GrowChecker();
                poolObjects.Add(pool[i]);
                count++;
            }
        }

        return null;
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
                currentActive++;
                GrowChecker();
                pool[i].gameObject.transform.position = position;
                poolObjects.Add(pool[i]);
                count++;
            }
        }

        return null;
    }

    /// <summary>
    /// Automatically grow the pool every time we use three out of four
    /// of the pool.
    /// </summary>
    void GrowChecker()
    {
        if (currentActive > threeOutOfFour)
        {
            threeOutOfFour += intendedSize * 3 / 4;

            for (int i = 0; i < intendedSize; i++)
            {
                PoolObject poolObject = new() { gameObject = GameObject.Instantiate(prefab) };

                handleCachedComponentRefs(poolObject);
                poolObject.gameObject.SetActive(false);

                pool.Add(poolObject);
            }
        }
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
