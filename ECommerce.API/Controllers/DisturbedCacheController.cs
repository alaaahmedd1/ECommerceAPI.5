using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Features.Products.Queries.GetProductById;
using ECommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

public sealed class DistributedCacheController : BaseApiController
{
    private readonly ICacheService<ProductResponse> _cacheService;
    private readonly IMediator _mediator;

    public DistributedCacheController(ICacheService<ProductResponse> cacheService, IMediator mediator)
    {
        _cacheService = cacheService;
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"productKey:{id}";

        var cachedProduct = await _cacheService.GetCacheAsync(cacheKey, cancellationToken);
        if (cachedProduct is not null)
        {
            return Ok(cachedProduct);
        }
        
        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);

        await _cacheService.SetCacheAsync(cacheKey, product, cancellationToken);

        return Ok(product);
    }
}