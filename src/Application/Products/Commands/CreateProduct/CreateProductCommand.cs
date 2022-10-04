using Application.Common.Interfaces;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommand : IRequest<Product>
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public float Weight { get; set; }
}

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product> 
{
    private readonly IKatapiDbContext _context;

    public CreateProductCommandHandler(IKatapiDbContext context)
    {
        _context = context;
    }
    
    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Product>();

        _context.Products.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}