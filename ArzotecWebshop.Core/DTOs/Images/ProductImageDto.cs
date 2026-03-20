namespace ArzotecWebshop.Core.DTOs.Images
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = "";
        public bool IsPrimary { get; set; }
        //public string OriginalFileName { get; set; } = "";
    }
}
