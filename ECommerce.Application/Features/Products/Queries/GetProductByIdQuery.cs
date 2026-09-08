using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductResponse>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _readRepository;

    public GetProductByIdQueryHandler(IProductRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _readRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        return new ProductResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Price,
            product.StockQuantity
        );
    }
}