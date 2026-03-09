using ArzotecWebshop.Core.DTOs;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
