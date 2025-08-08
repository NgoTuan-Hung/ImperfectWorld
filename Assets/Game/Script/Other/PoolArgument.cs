using System;

public enum ComponentType
{
    HealthAndManaIndicator,
    GameEffect,
    WorldSpaceUI,
    CustomMono,
};

public class PoolArgument
{
    public ComponentType componentType;

    public enum WhereComponent
    {
        Child,
        Self,
    }

    public WhereComponent whereComponent;

    public PoolArgument(ComponentType componentType, WhereComponent whereComponent)
    {
        this.componentType = componentType;
        this.whereComponent = whereComponent;
    }
}
