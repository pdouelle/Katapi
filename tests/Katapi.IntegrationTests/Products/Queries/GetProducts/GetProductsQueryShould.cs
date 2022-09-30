using Application.Products.Queries.GetProducts;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.IntegrationTests.XUnit.Products.Queries.GetProducts;

using static KatapiFactory;

public sealed class GetProductsQueryShould : BaseTestFixture
{
    private readonly Faker<Product> _productGenerator;

    public GetProductsQueryShould()
    {
        _productGenerator = new Faker<Product>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.Weight, f => f.Random.Float(1, 100));
    }

    [Fact]
    public async Task Ok()
    {
        List<Product> products = _productGenerator.Generate(3);

        await AddRangeAsync(products);

        var query = new GetProductsQuery();

        IEnumerable<Product> result = await SendAsync(query);

        result.Should().HaveCount(3);
    }
}