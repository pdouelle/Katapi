using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQuery : IRequest<Product?>
{
    [FromRoute(Name = ApiRoutes.Products.Id)]
    public Guid Id { get; set; }
}

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IKatapiDbContext _context;

    public GetProductByIdQueryHandler(IKatapiDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) =>
        await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken);
}