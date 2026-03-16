

namespace ArzotecWebshop.Core.DTOs.Import
{
    public class ProductImportDto
    {
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public int Available { get; set; }
        public int Purchased { get; set; }
        public string? Ean { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; } = "";
        public string Category { get; set; } = "";
    }
}
