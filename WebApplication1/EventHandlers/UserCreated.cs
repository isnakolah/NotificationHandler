namespace WebApplication1.EventHandlers;

[HandleEvent(Events.UserCreated)]
public class UserCreated : IEventHandler<User>
{
    public async Task HandleAsync(User data)
    {
        Console.WriteLine(data.FullName);
    }
}