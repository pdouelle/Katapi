using Application.Products.Commands.CreateProduct;
using Bogus;

namespace Application.IntegrationTests.XUnit.Fakers;

public sealed class CreateProductCommandFaker : Faker<CreateProductCommand>
{
    public CreateProductCommandFaker()
    {
        RuleFor(x => x.Name, f => f.Commerce.ProductName());
        RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()));
        RuleFor(x => x.Weight, f => f.Random.Float(1, 100));
    }
}