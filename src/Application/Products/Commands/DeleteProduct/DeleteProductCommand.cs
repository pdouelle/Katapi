using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommand : IRequest<Unit?>
{
    public Guid Id { get; set; }
}

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit?>
{
    private readonly IKatapiDbContext _context;

    public DeleteProductCommandHandler(IKatapiDbContext context)
    {
        _context = context;
    }

    public async Task<Unit?> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        Product? resource = await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (resource is null)
            return null;

        _context.Products.Remove(resource);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}