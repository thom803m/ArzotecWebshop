using ArzotecWebshop.Core.Models;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<ProductImage> UploadImageAsync(int productId, Stream fileStream, string fileName);
        Task SetPrimaryAsync(int productId, int imageId);
        Task DeleteImageAsync(int imageId);
        Task<ProductImage?> UploadFromUrlAsync(int productId, string imageUrl);
    }
}