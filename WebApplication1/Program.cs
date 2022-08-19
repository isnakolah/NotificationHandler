using WebApplication1.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.AddEventHandlers();

app.MapGet("api/test", async () =>
{
    await Task.Delay(3000);
});

app.Run();