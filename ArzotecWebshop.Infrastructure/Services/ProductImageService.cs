using ArzotecWebshop.Core.Interfaces.Services;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly AppDbContext _context;

        public ProductImageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductImage?> UploadFromUrlAsync(int productId, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new Exception("Product not found");

            var image = new ProductImage
            {
                ProductId = productId,
                Url = imageUrl.Trim(),
                IsPrimary = !product.Images.Any()
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }

        public async Task SetPrimaryAsync(int productId, int imageId)
        {
            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            var selected = images.FirstOrDefault(i => i.Id == imageId);
            if (selected == null)
                throw new Exception("Image not found");

            foreach (var img in images)
            {
                img.IsPrimary = img.Id == imageId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                throw new Exception("Image not found");

            _context.ProductImages.Remove(image);

            if (image.IsPrimary)
            {
                var next = await _context.ProductImages
                    .Where(i => i.ProductId == image.ProductId)
                    .OrderBy(i => i.Id)
                    .FirstOrDefaultAsync();

                if (next != null)
                    next.IsPrimary = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}