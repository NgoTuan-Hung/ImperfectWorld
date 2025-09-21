using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectComponent
{
    GameEffect,
    WorldSpaceUI,
}

public class PoolObject
{
    public GameObject gameObject;
    public HealthAndManaIndicator healthAndManaIndicator;
    public GameEffect gameEffect;
    public WorldSpaceUI worldSpaceUI;
    public CustomMono customMono;
    public IEnumerator idleScheme;
    public PoolRevampPoolObject poolRevampPoolObject;
    Dictionary<Type, Component> components = new();

    public void SetComponent<T>(T component)
        where T : Component
    {
        components[typeof(T)] = component;
    }

    // public GameEffect GameEffect => GetComponent<GameEffect>();
}
