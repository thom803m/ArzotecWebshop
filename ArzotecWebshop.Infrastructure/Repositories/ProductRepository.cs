using ArzotecWebshop.Core.DTOs.Common;
using ArzotecWebshop.Core.DTOs.Products;
using ArzotecWebshop.Core.Interfaces.Repositories;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArzotecWebshop.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository 
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Sku == sku);
        }

        public async Task<PagedResult<Product>> GetPagedAsync(ProductQueryParameters parameters)
        {
            var page = parameters.Page <= 0 ? 1 : parameters.Page;
            var pageSize = parameters.PageSize <= 0 ? 20 : parameters.PageSize;

            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = query.Where(p =>
                    p.Name.Contains(parameters.Search));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Brand))
            {
                query = query.Where(p =>
                    p.Brand.Name == parameters.Brand);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Category))
            {
                query = query.Where(p =>
                    p.Category.Name == parameters.Category);
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= parameters.MaxPrice.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
