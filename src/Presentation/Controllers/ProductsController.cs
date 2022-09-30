using Application.Products;
using Application.Products.Queries.GetProducts;
using Domain.Common;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public sealed class ProductsController : ApiControllerBase
{
    /// <summary>
    ///     Retrieves the collection of Product resources.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiRoutes.Products.GetList)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Get([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<Product> resources = await Mediator.Send(query, cancellationToken);

        var dto = resources.Adapt<List<ProductDto>>();

        return dto;
    }
}