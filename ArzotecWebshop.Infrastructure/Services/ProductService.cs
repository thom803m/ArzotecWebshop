using ArzotecWebshop.Core.DTOs.Common;
using ArzotecWebshop.Core.DTOs.Products;
using ArzotecWebshop.Core.Interfaces.Repositories;
using ArzotecWebshop.Core.Interfaces.Services;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class ProductService : IProductService 
    {
        private readonly IProductRepository _productRepository;
        private readonly AppDbContext _context;

        public ProductService(
            IProductRepository productRepository,
            AppDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Ean = product.Ean,
                Brand = product.Brand?.Name,
                Category = product.Category?.Name,
                Images = product.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
        {
            var result = await _productRepository.GetPagedAsync(parameters);

            return new PagedResult<ProductDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return null;

            return MapToDto(product);
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Name == dto.Brand);

            if (brand == null)
            {
                brand = new Brand { Name = dto.Brand };
                _context.Brands.Add(brand);
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == dto.Category);

            if (category == null)
            {
                category = new Category { Name = dto.Category };
                _context.Categories.Add(category);
            }

            var product = new Product
            {
                Sku = dto.Sku,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Ean = dto.Ean,
                Brand = brand,
                Category = category,
                LastSynced = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            Brand? brand = null;
            if (!string.IsNullOrWhiteSpace(dto.Brand))
            {
                brand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.Name == dto.Brand);

                if (brand == null)
                {
                    brand = new Brand { Name = dto.Brand.Trim() };
                    _context.Brands.Add(brand);
                }
            }

            Category? category = null;
            if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == dto.Category);

                if (category == null)
                {
                    category = new Category { Name = dto.Category.Trim() };
                    _context.Categories.Add(category);
                }
            }

            product.Sku = dto.Sku;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.Ean = dto.Ean;
            product.Brand = brand;
            product.Category = category;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }
    }
}
