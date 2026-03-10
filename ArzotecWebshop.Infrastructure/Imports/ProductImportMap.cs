using ArzotecWebshop.Core.DTOs;
using CsvHelper.Configuration;

namespace ArzotecWebshop.Infrastructure.Imports
{
    public class ProductImportMap : ClassMap<ProductImportDto>
    {
        public ProductImportMap()
        {
            Map(m => m.Sku).Name("SKU", "sku", "ProductNumber");
            Map(m => m.Name).Name("Name", "ProductName");
            Map(m => m.Price).Name("Price", "SalesPrice");
            Map(m => m.StockQuantity).Name("Stock", "Quantity");
            Map(m => m.Brand).Name("Brand", "BrandName");
            Map(m => m.Category).Name("Category", "CategoryName");
        }
    }
}
