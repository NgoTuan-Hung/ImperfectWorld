using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectPool
{
    public List<PoolObject> pool;
    public int size;

    public ObjectPool(GameObject prefab, int size, params PoolArgument[] poolArguments)
    {
        this.size = size;

        pool = new List<PoolObject>(size);
        for (int i=0;i<pool.Capacity;i++)
        {
            PoolObject poolObject = new PoolObject
            {
                GameObject = GameObject.Instantiate(prefab)
            };

            pool.Add(poolObject);
        }

        if (poolArguments.Length > 0)
        {   
            for (int i=0;i<pool.Capacity;i++)
            {
                foreach (PoolArgument poolArgument in poolArguments)
                {
                    switch (poolArgument.whereComponent)
                    {
                        case PoolArgument.WhereComponent.Child:
                            if (poolArgument.Type == typeof(GameObject)) break; 
                            else
                            {
                                foreach (FieldInfo fieldInfo in typeof(PoolObject).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                                {   
                                    if (fieldInfo.FieldType == poolArgument.Type)
                                    {
                                        fieldInfo.SetValue(pool[i], pool[i].GameObject.GetComponentInChildren(poolArgument.Type));
                                    }
                                }
                            }

                            break;
                        case PoolArgument.WhereComponent.Self:
                            if (poolArgument.Type == typeof(GameObject)) break;
                            else 
                            {
                                foreach (FieldInfo fieldInfo in typeof(PoolObject).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                                {
                                    if (fieldInfo.FieldType == poolArgument.Type)
                                    {
                                        fieldInfo.SetValue(pool[i], pool[i].GameObject.GetComponent(poolArgument.Type));
                                    }
                                }
                            }
                            
                            break;
                    }
                }

                pool[i].GameObject.SetActive(false);
            }
        }
    }

    public PoolObject PickOne()
    {
        for (int i=0;i<pool.Count;i++)
        {
            if (!pool[i].GameObject.activeSelf)
            {
                pool[i].GameObject.SetActive(true);
                return pool[i];
            }
        }

        return null;
    }

    public List<PoolObject> Pick(int n)
    {
        int count = 0;
        List<PoolObject> poolObjects = new List<PoolObject>();
        for (int i=0;i<pool.Count;i++)
        {
            if (count >= n) return poolObjects;

            if (!pool[i].GameObject.activeSelf)
            {
                pool[i].GameObject.SetActive(true);
                poolObjects.Add(pool[i]);
                count++;
            }
        }

        return null;
    }
}