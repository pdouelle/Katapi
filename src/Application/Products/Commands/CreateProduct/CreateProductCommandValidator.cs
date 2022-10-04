using FluentValidation;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3);
        
        RuleFor(x => x.Price)
            .NotEmpty();
        
        RuleFor(x => x.Weight)
            .NotEmpty();
    }
}