namespace WebApplication1.EventHandlers;

[HandleEvent(Events.CompanyCreated)]
public class CompanyCreated : IEventHandler<Company>
{
    private readonly ILogger<CompanyCreated> _logger;
    private readonly HttpClient _client;

    public CompanyCreated(ILogger<CompanyCreated> logger, IHttpClientFactory clientFactory)
    {
        (_logger, _client) = (logger, clientFactory.CreateClient());
    }

    public async Task HandleAsync(Company data)
    {
        await _client.GetAsync("https://localhost:7292/api/test").ConfigureAwait(false);

        _logger.LogInformation("The company name is: {Name}", data.Name);
    }
}