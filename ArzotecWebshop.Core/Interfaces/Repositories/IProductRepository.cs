using ArzotecWebshop.Core.Models;

namespace ArzotecWebshop.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetBySkuAsync(string sku);
        Task AddAsync (Product product);
        Task SaveChangesAsync();
    }
}
