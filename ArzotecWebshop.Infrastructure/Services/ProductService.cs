using ArzotecWebshop.Core.DTOs;
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
    }
}
