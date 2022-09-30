using Application.Products;
using Application.Products.Queries;
using Domain.Common;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public sealed class ProductsController : ApiControllerBase
{
    [HttpGet(ApiRoutes.Products.GetList)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Get(GetProductsQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<Product> resources = await Mediator.Send(query, cancellationToken);

        var dto = resources.Adapt<List<ProductDto>>();

        return dto;
    }
}