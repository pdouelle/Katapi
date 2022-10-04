using Application.Common.Interfaces;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommand : IRequest<Product?>
{
    public Guid Id { get; set; }
}

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product?>
{
    private readonly IKatapiDbContext _context;

    public UpdateProductCommandHandler(IKatapiDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product? resource = await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (resource is null)
            return resource;

        request.Adapt(resource);

        _context.Products.Update(resource);

        await _context.SaveChangesAsync(cancellationToken);

        return resource;
    }
}