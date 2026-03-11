

namespace ArzotecWebshop.Core.DTOs.Products
{
    public class ProductQueryParameters
    {
        public string? Search { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
