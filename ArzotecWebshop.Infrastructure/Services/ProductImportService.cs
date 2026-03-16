using ArzotecWebshop.Core.DTOs.Import;
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
        private readonly AppDbContext _context;

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

            var config = new CsvConfiguration(new CultureInfo("dk-DK"))
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<ProductImportMap>();

            var records = csv.GetRecords<ProductImportDto>().ToList();

            var existingProducts = await _context.Products
                .ToDictionaryAsync(p => p.Sku);

            foreach (var r in records)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(r.Sku))
                    {
                        result.Skipped++;
                        continue;
                    }

                    var sku = NormalizeScientific(r.Sku);
                    var ean = NormalizeScientific(r.Ean);

                    existingProducts.TryGetValue(sku, out var existing);
                    var stock = CalculateStock(r);

                    Brand? brand = null;
                    if (!string.IsNullOrWhiteSpace(r.Brand))
                    {
                        brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == r.Brand.Trim());
                        if (brand == null)
                        {
                            brand = new Brand { Name = r.Brand.Trim() };
                            await _context.Brands.AddAsync(brand);
                            await _context.SaveChangesAsync();
                        }
                    }

                    Category? category = null;
                    if (!string.IsNullOrWhiteSpace(r.Category))
                    {
                        category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == r.Category.Trim());
                        if (category == null)
                        {
                            category = new Category { Name = r.Category.Trim() };
                            await _context.Categories.AddAsync(category);
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (existing == null)
                    {
                        var product = new Product
                        {
                            Sku = sku,
                            Name = r.Name?.Trim() ?? "",
                            Price = r.Price,
                            StockQuantity = stock,
                            Ean = ean,
                            BrandId = brand?.Id,
                            CategoryId = category?.Id,
                            LastSynced = DateTime.UtcNow
                        };

                        await _productRepository.AddAsync(product);
                        existingProducts[sku] = product;

                        result.Created++;
                    }
                    else
                    {
                        existing.Name = r.Name?.Trim() ?? "";
                        existing.Price = r.Price;
                        existing.StockQuantity = stock;
                        existing.Ean = ean;
                        existing.BrandId = brand?.Id;
                        existing.CategoryId = category?.Id;
                        existing.LastSynced = DateTime.UtcNow;

                        result.Updated++;
                    }
                }
                catch
                {
                    result.Skipped++;
                }
            }

            await _productRepository.SaveChangesAsync();
            return result;
        }

        private static int CalculateStock(ProductImportDto record)
        {
            return record.Available + record.Purchased;
        }

        private static string NormalizeScientific(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            value = value.Trim();

            if (value.Contains("E+") || value.Contains("e+"))
            {
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
                {
                    return ((long)number).ToString();
                }
            }

            return value;
        }
    }
}