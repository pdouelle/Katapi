using Bogus;
using Domain.Entities;

namespace Application.IntegrationTests.XUnit.Fakers;

public sealed class ProductFaker : Faker<Product> {
    public ProductFaker()
    {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.Name, f => f.Commerce.ProductName());
        RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()));
        RuleFor(x => x.Weight, f => f.Random.Float(1, 100));
    }
}