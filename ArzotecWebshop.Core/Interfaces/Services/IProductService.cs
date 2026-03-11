using ArzotecWebshop.Core.DTOs.Common;
using ArzotecWebshop.Core.DTOs.Products;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);
    }
}
