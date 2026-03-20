using ArzotecWebshop.Core.DTOs.Brands;
using ArzotecWebshop.Core.Interfaces.Services;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;

        public BrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BrandDto>> GetBrandsAsync()
        {
            return await _context.Brands
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(b => new BrandDto { Id = b.Id, Name = b.Name })
                .ToListAsync();
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null) 
                return null;

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name cannot be empty");

            var brand = new Brand { Name = name };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<BrandDto?> UpdateBrandAsync(int id, UpdateBrandDto dto)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null) 
                return null;

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name cannot be empty");

            brand.Name = name;

            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null) 
                return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
