using ArzotecWebshop.Core.DTOs;
using ArzotecWebshop.Core.Interfaces.Repositories;
using ArzotecWebshop.Core.Interfaces.Services;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Data;
using ArzotecWebshop.Infrastructure.Imports;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class ProductImportService : IProductImportService
    {
        private readonly IProductRepository _productRepository;
        private AppDbContext _context;

        public ProductImportService(
            IProductRepository productRepository,
            AppDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<ImportResult> ImportCsvAsync(Stream filestream)
        {
            var result = new ImportResult();
            
            using var reader = new StreamReader(filestream);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";"
            };

            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ProductImportMap>();

            var records = csv.GetRecords<ProductImportDto>();

            foreach (var r in records)
            {
                var brand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.Name == r.Brand);

                if (brand == null)
                {
                    brand = new Brand { Name = r.Brand };
                    _context.Brands.Add(brand);
                }

                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == r.Category);

                if (category == null)
                {
                    category = new Category { Name = r.Category };
                    _context.Categories.Add(category);
                }

                var existing = await _productRepository.GetBySkuAsync(r.Sku);

                if (existing == null)
                {
                    var product = new Product
                    {
                        Sku = r.Sku,
                        Name = r.Name,
                        Price = r.Price,
                        StockQuantity = r.StockQuantity,
                        Brand = brand,
                        Category = category,
                        LastSynced = DateTime.UtcNow
                    };

                    await _productRepository.AddAsync(product);
                    result.Created++;
                }
                else
                {
                    existing.Name = r.Name;
                    existing.Price = r.Price;
                    existing.StockQuantity = r.StockQuantity;
                    existing.Brand = brand;
                    existing.Category = category;
                    existing.LastSynced = DateTime.UtcNow;
                    result.Updated++;
                }
            }

            await _productRepository.SaveChangesAsync();

            return result;
        }
    }
}
