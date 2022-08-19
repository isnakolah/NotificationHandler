namespace WebApplication1.EventHandlers;

[HandleEvent(Events.InvoiceCreated)]
public class InvoiceCreated : IEventHandler<Invoice>
{
    public async Task HandleAsync(Invoice data)
    {
        await Task.Delay(1);

        Console.WriteLine($"The invoice number is : {data.InvoiceNo} and total is {data.Total}");
    }
}