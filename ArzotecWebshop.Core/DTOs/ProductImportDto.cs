

namespace ArzotecWebshop.Core.DTOs
{
    public class ProductImportDto
    {
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Brand { get; set; } = "";
        public string Category { get; set; } = "";
    }
}
