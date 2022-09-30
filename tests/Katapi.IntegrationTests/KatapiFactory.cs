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

public sealed class KatapiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static IServiceScopeFactory _scopeFactory = null!;
    private static PostgreSqlTestcontainer _dbContainer = null!;
    private static Checkpoint _checkpoint = null!;

    public async Task InitializeAsync()
    {
        _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres",
            }).Build();

        await _dbContainer.StartAsync();

        _scopeFactory = Services.GetRequiredService<IServiceScopeFactory>();

        _checkpoint = new Checkpoint
        {
            SchemasToInclude = new[]
            {
                "public"
            },
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
        };
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_dbContainer.ConnectionString));
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
        var defaultConnection = _dbContainer.ConnectionString;

        await using var conn = new NpgsqlConnection(defaultConnection);

        await conn.OpenAsync();

        await _checkpoint.Reset(conn);
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }
    
    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }
    
    public static async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.AddRange(entities);

        await context.SaveChangesAsync();
    }
    
    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}