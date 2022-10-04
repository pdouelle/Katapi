using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Persistance;
using Presentation;
using Respawn;
using Respawn.Graph;
using Xunit;

namespace Application.IntegrationTests.XUnit;

public sealed class KatapiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private static IServiceScopeFactory _scopeFactory = null!;
    private static PostgreSqlTestcontainer _container = null!;
    private static Checkpoint _checkpoint = null!;

    public async Task InitializeAsync()
    {
        _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres",
            }).Build();

        await _container.StartAsync();

        _scopeFactory = Services.GetRequiredService<IServiceScopeFactory>();

        _checkpoint = new Checkpoint
        {
            SchemasToInclude = new[]
            {
                "public"
            },
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new Table[] { "__EFMigrationsHistory" },
        };
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<KatapiDbContext>));
            services.AddDbContext<KatapiDbContext>(options => options.UseNpgsql(_container.ConnectionString));
        });
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task ResetState()
    {
        await using var connection = new NpgsqlConnection(_container.ConnectionString);

        await connection.OpenAsync();

        await _checkpoint.Reset(connection);
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<KatapiDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<KatapiDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<KatapiDbContext>();

        context.AddRange(entities);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<KatapiDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    public new async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}