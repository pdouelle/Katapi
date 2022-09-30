using Application.Products.Queries;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.IntegrationTests.XUnit.Products.Queries;

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
    public async Task Test1()
    {
        List<Product>? products = _productGenerator.Generate(3);
        
        await AddRangeAsync(products);
        
        var query = new GetProductsQuery();

        IEnumerable<Product> result = await SendAsync(query);

        result.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task Test2()
    {
        List<Product>? products = _productGenerator.Generate(2);
        
        await AddRangeAsync(products);
        
        var query = new GetProductsQuery();

        IEnumerable<Product> result = await SendAsync(query);

        result.Should().HaveCount(2);
    }
}
