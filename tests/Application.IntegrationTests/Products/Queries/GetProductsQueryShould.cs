using Application.Products.Queries;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Products.Queries;

using static Testing;

public class GetProductsQueryShould : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnOk()
    {
        List<Product> products = new Faker<Product>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.Weight, f => f.Random.Float(1, 100))
            .Generate(3)
            .ToList();
        
        await AddRangeAsync(products);
        
        var query = new GetProductsQuery();

        IEnumerable<Product> result = await SendAsync(query);

        result.Should().HaveCount(3);
    }
}
