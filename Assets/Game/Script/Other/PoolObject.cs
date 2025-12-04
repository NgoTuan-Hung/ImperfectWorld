using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectComponent
{
    HealthAndManaIndicator,
    GameEffect,
    WorldSpaceUI,
    CustomMono,
    TextPopupUI,
    Item,
}

public class PoolObject
{
    public GameObject gameObject;
    public IEnumerator idleScheme;
    public PoolRevampPoolObject poolRevampPoolObject;
    Dictionary<PoolObjectComponent, Component> components = new();

    T GetComponent<T>(PoolObjectComponent p_pOC)
        where T : Component
    {
        return components[p_pOC] as T;
    }

    public void AddComponent(PoolObjectComponent p_pOC, Component p_component) =>
        components.Add(p_pOC, p_component);

    public HealthAndManaIndicator HealthAndManaIndicator =>
        GetComponent<HealthAndManaIndicator>(PoolObjectComponent.HealthAndManaIndicator);
    public GameEffect GameEffect => GetComponent<GameEffect>(PoolObjectComponent.GameEffect);
    public WorldSpaceUI WorldSpaceUI =>
        GetComponent<WorldSpaceUI>(PoolObjectComponent.WorldSpaceUI);
    public CustomMono CustomMono => GetComponent<CustomMono>(PoolObjectComponent.CustomMono);
    public TextPopupUI TextPopupUI => GetComponent<TextPopupUI>(PoolObjectComponent.TextPopupUI);
    public Item Item => GetComponent<Item>(PoolObjectComponent.Item);
}
