namespace AnonymousChatApi.Abstractions;

public interface IEvent<out T>
{
    public T Body { get; }
}