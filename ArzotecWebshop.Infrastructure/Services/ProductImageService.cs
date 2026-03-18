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
        private readonly string _basePath;

        public ProductImageService(AppDbContext context)
        {
            _context = context;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public async Task<ProductImage> UploadImageAsync(int productId, Stream fileStream, string fileName)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new Exception("Product not found");

            var folderPath = Path.Combine(_basePath, "images", "products", productId.ToString());

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(fileName);
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            var isPrimary = !product.Images.Any();

            var image = new ProductImage
            {
                ProductId = productId,
                Url = $"/images/products/{productId}/{newFileName}",
                IsPrimary = isPrimary,
                //OriginalFileName = fileName
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

            var productId = image.ProductId;
            var wasPrimary = image.IsPrimary;

            var fullPath = Path.Combine(_basePath, image.Url.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            if (wasPrimary)
            {
                var nextImage = await _context.ProductImages
                    .Where(i => i.ProductId == productId)
                    .OrderBy(i => i.Id)
                    .FirstOrDefaultAsync();

                if (nextImage != null)
                {
                    nextImage.IsPrimary = true;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<ProductImage?> UploadFromUrlAsync(int productId, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(imageUrl);
                if (!response.IsSuccessStatusCode)
                    return null;

                await using var stream = await response.Content.ReadAsStreamAsync();
                var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);

                return await UploadImageAsync(productId, stream, fileName);
            }
            catch
            {
                // Log evt. fejl her
                return null;
            }
        }
    }
}