using ArzotecWebshop.Core.Models;

namespace ArzotecWebshop.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetProductSkuAsync(string sku);
        Task AddAsync (Product product);
        Task SaveChangesAsync();
    }
}
