using Microsoft.EntityFrameworkCore;

namespace ArzotecWebshop.Core.Models
{
    [Index(nameof(Sku), IsUnique = true)]
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Ean { get; set; }
        public DateTime LastSynced { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
