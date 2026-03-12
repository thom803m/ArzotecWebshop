using ArzotecWebshop.Core.DTOs.Common;
using ArzotecWebshop.Core.DTOs.Products;
using ArzotecWebshop.Core.Models;

namespace ArzotecWebshop.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetBySkuAsync(string sku);
        Task<PagedResult<Product>> GetPagedAsync(ProductQueryParameters parameters);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync (Product product);
        void Update (Product product);
        void Delete (Product product);
        Task SaveChangesAsync();
    }
}
