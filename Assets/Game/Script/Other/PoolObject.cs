using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    public GameObject gameObject;
    public IEnumerator idleScheme;
    Dictionary<ComponentType, Component> components = new();

    T GetComponent<T>(ComponentType p_pOC)
        where T : Component
    {
        return components[p_pOC] as T;
    }

    public void AddComponent(ComponentType p_pOC, Component p_component) =>
        components.Add(p_pOC, p_component);

    public HealthAndManaIndicator HealthAndManaIndicator =>
        GetComponent<HealthAndManaIndicator>(ComponentType.HealthAndManaIndicator);
    public GameEffect GameEffect => GetComponent<GameEffect>(ComponentType.GameEffect);
    public WorldSpaceUI WorldSpaceUI => GetComponent<WorldSpaceUI>(ComponentType.WorldSpaceUI);
    public CustomMono CustomMono => GetComponent<CustomMono>(ComponentType.CustomMono);
    public TextPopupUI TextPopupUI => GetComponent<TextPopupUI>(ComponentType.TextPopupUI);
    public Item Item => GetComponent<Item>(ComponentType.Item);
    public ChampionRewardUI ChampionRewardUI =>
        GetComponent<ChampionRewardUI>(ComponentType.ChampionRewardUI);
    public Relic Relic => GetComponent<Relic>(ComponentType.Relic);
    public BasicUI BasicUI => GetComponent<BasicUI>(ComponentType.BasicUI);
}
