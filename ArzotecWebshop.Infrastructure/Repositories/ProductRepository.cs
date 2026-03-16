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
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Sku == sku);
        }

        public async Task<PagedResult<Product>> GetPagedAsync(ProductQueryParameters parameters)
        {
            var page = parameters.Page <= 0 ? 1 : parameters.Page;
            var pageSize = parameters.PageSize <= 0 ? 1000 : parameters.PageSize;

            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .OrderBy(p => p.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var searchTerm = parameters.Search.Trim();
                var search = $"%{searchTerm}%";

                query = query.Where(p =>
                    EF.Functions.Like(p.Name, search) ||
                    EF.Functions.Like(p.Sku, search) ||
                    EF.Functions.Like(p.Ean, search) ||
                    (p.Brand != null && EF.Functions.Like(p.Brand.Name, search)) ||
                    (p.Category != null && EF.Functions.Like(p.Category.Name, search)));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Brand))
            {
                query = query.Where(p =>
                    p.Brand != null && p.Brand.Name == parameters.Brand);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Category))
            {
                query = query.Where(p =>
                    p.Category != null && p.Category.Name == parameters.Category);
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

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
