using Application.Products;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using Domain.Common;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

using static ApiRoutes;

public sealed class ProductsController : ApiControllerBase
{
    /// <summary>
    ///     Retrieves the collection of Product resources.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Products.GetList)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Get([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<Product> resources = await Mediator.Send(query, cancellationToken);

        return resources.Adapt<List<ProductDto>>();
    }

    /// <summary>
    ///     Retrieves a Product resource.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Products.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById([FromQuery] GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Product? resource = await Mediator.Send(request, cancellationToken);

        if (resource is null)
            return NotFound();

        return resource.Adapt<ProductDto>();
    }

    /// <summary>
    ///     Create a Product resource.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost(Products.Post)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post([FromBody] CreateProductCommand request, CancellationToken cancellationToken)
    {
        Product resource = await Mediator.Send(request, cancellationToken);

        var dto = resource.Adapt<ProductDto>();

        return CreatedAtAction(nameof(GetById), new { dto.Id }, dto);
    }

    /// <summary>
    ///     Replace the Product resource.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut(Products.Put)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Put([FromRoute(Name = Products.Id)] Guid id, [FromBody] UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        Product? resource = await Mediator.Send(request, cancellationToken);

        if (resource is null)
            return NotFound();

        return resource.Adapt<ProductDto>();
    }

    /// <summary>
    ///     Updates the Product resource.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="patchDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch(Products.Patch)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Patch
    (
        [FromRoute(Name = Products.Id)]
        Guid id,
        [FromBody] JsonPatchDocument<UpdateProductCommand> patchDocument,
        CancellationToken cancellationToken
    )
    {
        Product? resource = await Mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken);

        if (resource is null)
            return NotFound();

        var model = resource.Adapt<UpdateProductCommand>();

        patchDocument.ApplyTo(model);

        return await Put(id, model, cancellationToken);
    }
    
    /// <summary>
    ///     Removes the Product resource.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete(Products.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);

        return NoContent();
    }
}