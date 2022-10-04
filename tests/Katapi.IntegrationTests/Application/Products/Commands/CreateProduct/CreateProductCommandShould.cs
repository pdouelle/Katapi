using Application.IntegrationTests.XUnit.Fakers;
using Application.Products.Commands.CreateProduct;
using Bogus;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace Application.IntegrationTests.XUnit.Application.Products.Commands.CreateProduct;

using static KatapiFactory;

public sealed class CreateProductCommandShould : BaseTestFixture
{
    private readonly Faker<CreateProductCommand> _createProductCommandGenerator = new CreateProductCommandFaker();

    public CreateProductCommandShould(KatapiFactory factory) : base(factory)
    {
        Client.BaseAddress = new Uri(Client.BaseAddress!, ApiRoutes.Products.Post);
    }

    [Fact]
    public async Task CreateProduct()
    {
        // Arrange
        CreateProductCommand command = _createProductCommandGenerator.Generate();

        // Act
        Guid id = (await SendAsync(command)).Id;

        // Assert
        var resource = await FindAsync<Product>(id);
        resource.Should().NotBeNull();
        resource.Should().BeEquivalentTo(command);
    }

    [Fact]
    public async Task ThrowValidationExceptionWhenNameLengthLower3()
    {
        // Arrange
        CreateProductCommand command = _createProductCommandGenerator.Generate();
        command.Name = "Aq";

        // Act & Assert
        await FluentActions.Invoking(() => SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>()
            .Where(x => x.Errors.SingleOrDefault()!.ErrorMessage == "The length of 'Name' must be at least 3 characters. You entered 2 characters.");
    }
    
    [Fact]
    public async Task ThrowValidationExceptionWhenPriceIsEmpty()
    {
        // Arrange
        CreateProductCommand command = _createProductCommandGenerator.Generate();
        command.Price = 0;

        // Act & Assert
        await FluentActions.Invoking(() => SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>()
            .Where(x => x.Errors.SingleOrDefault()!.ErrorMessage == "'Price' must not be empty.");
    }
    
    [Fact]
    public async Task ThrowValidationExceptionWhenWeightIsEmpty()
    {
        // Arrange
        CreateProductCommand command = _createProductCommandGenerator.Generate();
        command.Weight = 0;

        // Act & Assert
        await FluentActions.Invoking(() => SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>()
            .Where(x => x.Errors.SingleOrDefault()!.ErrorMessage == "'Weight' must not be empty.");
    }
}