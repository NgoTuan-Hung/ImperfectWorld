using System;

public class PoolArgument
{
    private Type type;
    public enum WhereComponent {Child, Self}
    public WhereComponent whereComponent;

    public Type Type { get => type; set => type = value; }

    public PoolArgument(Type type, WhereComponent whereComponent)
    {
        this.type = type;
        this.whereComponent = whereComponent;
    }
}