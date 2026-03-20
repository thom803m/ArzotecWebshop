using ArzotecWebshop.Core.Models;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<ProductImage?> UploadFromUrlAsync(int productId, string imageUrl);
        Task SetPrimaryAsync(int productId, int imageId);
        Task DeleteImageAsync(int imageId);
    }
}