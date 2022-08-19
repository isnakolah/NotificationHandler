namespace WebApplication1.Common.Interfaces;

public interface IEventHandler<in T>
{
    Task HandleAsync(T data);
}