using Application.IntegrationTests.XUnit.Fakers;
using Application.Products.Queries.GetProducts;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.IntegrationTests.XUnit.Application.Products.Queries.GetProducts;

using static KatapiFactory;

public sealed class GetProductsQueryShould : BaseTestFixture
{
    private readonly Faker<Product> _productGenerator = new ProductFaker();

    public GetProductsQueryShould(KatapiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ReturnProducts()
    {
        List<Product> products = _productGenerator.Generate(3);

        await AddRangeAsync(products);

        var query = new GetProductsQuery();

        IEnumerable<Product> result = await SendAsync(query);

        result.Should().HaveCount(3);
    }
}