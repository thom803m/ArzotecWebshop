

namespace ArzotecWebshop.Core.DTOs.Products
{
    public class CreateProductDto
    {
        public string Sku { get; set; } = "";

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string Brand { get; set; } = "";

        public string Category { get; set; } = "";
    }
}
