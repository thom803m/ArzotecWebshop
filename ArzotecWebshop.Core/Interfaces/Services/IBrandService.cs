using ArzotecWebshop.Core.DTOs.Brands;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int id);
        Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);
        Task<BrandDto?> UpdateBrandAsync(int id, UpdateBrandDto dto);
        Task<bool> DeleteBrandAsync(int id);
    }
}
