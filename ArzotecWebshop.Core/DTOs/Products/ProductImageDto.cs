

namespace ArzotecWebshop.Core.DTOs.Products
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        //public string OriginalFileName { get; set; } = string.Empty;
    }
}
