using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProducts;

public sealed class GetProductsQuery : IRequest<IEnumerable<Product>>
{
}

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
{
    private readonly IKatapiDbContext _context;

    public GetProductsQueryHandler(IKatapiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
        await _context.Products.AsNoTracking()
            .ToListAsync(cancellationToken);
}