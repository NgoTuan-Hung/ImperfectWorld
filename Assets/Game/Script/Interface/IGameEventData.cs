public interface IGameEventData
{
    public T As<T>()
        where T : IGameEventData => (T)this;
}
