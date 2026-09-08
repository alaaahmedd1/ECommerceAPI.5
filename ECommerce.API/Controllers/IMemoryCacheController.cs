using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.API.Controllers;

public sealed class InMemoryCacheController : BaseApiController
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMediator _mediator;

    public InMemoryCacheController(IMemoryCache memoryCache, IMediator mediator)
    {
        _memoryCache = memoryCache;
        _mediator = mediator;
    }

    [HttpGet("InMemoryGet")]
    public async Task<ActionResult<ProductResponse>> Get(int key, CancellationToken cancellationToken)
    {
        var product = await _memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _mediator.Send(new GetProductByIdQuery(key), cancellationToken);
        });

        return Ok(product);
    }

    [HttpPost("InMemorySet")]
    public IActionResult Set(int key, ProductResponse product)
    {
        _memoryCache.Set(key, product);
        return Ok();
    }
}