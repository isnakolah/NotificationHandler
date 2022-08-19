namespace WebApplication1.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HandleEventAttribute : Attribute
{
    public HandleEventAttribute(string eventName)
    {
        EventName = eventName;
    }

    public string EventName { get; }
}