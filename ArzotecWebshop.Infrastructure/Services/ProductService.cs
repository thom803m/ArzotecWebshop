using ArzotecWebshop.Core.DTOs.Common;
using ArzotecWebshop.Core.DTOs.Products;
using ArzotecWebshop.Core.Interfaces.Repositories;
using ArzotecWebshop.Core.Interfaces.Services;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class ProductService : IProductService 
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(p => new ProductDto
            {
                Sku = p.Sku,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Brand = p.Brand?.Name,
                Category = p.Category?.Name
            }).ToList();
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
        {
            var result = await _productRepository.GetPagedAsync(parameters);

            var dto = result.Items.Select(p => new ProductDto
            {
                Sku = p.Sku,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Brand = p.Brand?.Name,
                Category = p.Category?.Name
            }).ToList();

            return new PagedResult<ProductDto>
            {
                Items = dto,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
