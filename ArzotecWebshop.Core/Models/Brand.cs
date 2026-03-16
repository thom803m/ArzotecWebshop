using Microsoft.EntityFrameworkCore;

namespace ArzotecWebshop.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
