using Xunit;

namespace Application.IntegrationTests.XUnit;

using static KatapiFactory;

public abstract class BaseTestFixture : IClassFixture<KatapiFactory>, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await ResetState();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}