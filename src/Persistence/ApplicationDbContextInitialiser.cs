using Bogus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistance;

public sealed class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!_context.Products.Any())
        {
            List<Product> products = new Faker<Product>()
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(x => x.Weight, f => f.Random.Float(1, 100))
                .Generate(3);

            _context.Products.AddRange(products);

            await _context.SaveChangesAsync();
        }
    }
}