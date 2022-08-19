using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Common.Extensions;

public record CallbackRequest
{
    public string Event { get; init; }
    public string EventType { get; init; }
    public object Data { get; init; }
}

public static class EventHandlers
{
    private static IImmutableDictionary<string, (Type HandlerType, Type DataType)>? _eventHandlers;

    public static WebApplication AddEventHandlers(this WebApplication app)
    {
        _eventHandlers ??= GetEventHandlers();

        app.MapPost("api/callback", async ([FromBody] CallbackRequest request) =>
        {
            var (handlerType, dataType) = GetEventHandler(request.EventType);

            var data = JsonSerializer.Deserialize(
                request.Data.ToString()!,
                dataType,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            var constructor = handlerType
                .GetConstructors()
                .SingleOrDefault(x => x.IsPublic && !x.IsStatic && !x.IsAbstract && !x.IsVirtual);

            var constructorParams = constructor?.GetParameters();

            object? handlerInstance;

            if (constructorParams?.Any() ?? false)
            {
                var scope = app.Services.CreateAsyncScope();

                var injectedServices = constructorParams.Select(x => scope.ServiceProvider.GetRequiredService(x.ParameterType));

                handlerInstance = Activator.CreateInstance(handlerType, injectedServices.ToArray());
            }
            else
            {
                handlerInstance = Activator.CreateInstance(handlerType);
            }

            var methodInfo = handlerType.GetMethod("HandleAsync");

            await (Task) methodInfo!.Invoke(handlerInstance, new[] {data})!;
        });

        return app;
    }

    private static (Type HandlerType, Type DataType) GetEventHandler(string eventName)
    {
        return _eventHandlers![eventName];
    }

    private static IImmutableDictionary<string, (Type HandlerType, Type DataType)> GetEventHandlers()
    {
        var eventHandlers = new Dictionary<string, (Type HandlerType, Type DataType)>();

        var assembly = Assembly.GetExecutingAssembly();

        var handlers = assembly
            .GetExportedTypes()
            .Where(t => t.GetCustomAttribute<HandleEventAttribute>() is not null);

        foreach (var handler in handlers)
        {
            if (handler.GetInterface("IEventHandler`1") is null)
                throw new Exception($"Handler {handler.Name} must implement interface");

            var attr = handler.GetCustomAttribute<HandleEventAttribute>();

            var eventName = attr!.EventName;

            var dataType = handler.GetInterface("IEventHandler`1")!.GetGenericArguments().First();

            if (!eventHandlers.TryAdd(eventName, (handler, dataType)))
                throw new Exception($"An error occured while trying to add event {eventName}");
        }

        return eventHandlers.ToImmutableDictionary();
    }
}