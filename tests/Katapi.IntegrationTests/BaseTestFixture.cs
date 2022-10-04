using Domain.Common;
using Xunit;

namespace Application.IntegrationTests.XUnit;

using static KatapiFactory;

public abstract class BaseTestFixture : IClassFixture<KatapiFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;

    protected BaseTestFixture(KatapiFactory factory)
    {
        Client = factory.CreateClient();
        Client.BaseAddress = new Uri(Client.BaseAddress!, ApiRoutes.Root);
    }

    public async Task InitializeAsync()
    {
        await ResetState();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}