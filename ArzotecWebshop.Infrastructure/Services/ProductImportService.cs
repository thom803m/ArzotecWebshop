using ArzotecWebshop.Core.DTOs.Import;
using ArzotecWebshop.Core.Interfaces.Repositories;
using ArzotecWebshop.Core.Interfaces.Services;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Data;
using ArzotecWebshop.Infrastructure.Imports;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Xml.Linq;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class ProductImportService : IProductImportService
    {
        private readonly IProductRepository _productRepository;
        private readonly AppDbContext _context;
        private readonly IProductImageService _productImageService;

        public ProductImportService(
            IProductRepository productRepository,
            AppDbContext context,
            IProductImageService productImageService)
        {
            _productRepository = productRepository;
            _context = context;
            _productImageService = productImageService;
        }

        public async Task<ImportResult> ImportCsvAsync(Stream filestream)
        {
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
            return await ImportRecordsAsync(records);
        }

        public async Task<ImportResult> ImportXmlAsync(Stream filestream)
        {
            var doc = XDocument.Load(filestream);
            var records = doc.Descendants("Product")
                .Select(x => new ProductImportDto
                {
                    Sku = (string)x.Element("Sku") ?? "",
                    Name = (string)x.Element("Name") ?? "",
                    Available = (int?)x.Element("Available") ?? 0,
                    Purchased = (int?)x.Element("Purchased") ?? 0,
                    Ean = (string)x.Element("Ean"),
                    Price = (decimal?)x.Element("Price") ?? 0,
                    Brand = (string)x.Element("Brand") ?? "",
                    Category = (string)x.Element("Category") ?? "",
                    ImageUrl = (string)x.Element("ImageUrl")
                })
                .ToList();

            return await ImportRecordsAsync(records);
        }

        private async Task<ImportResult> ImportRecordsAsync(List<ProductImportDto> records)
        {
            var result = new ImportResult();
            var existingProducts = await _context.Products
                .ToDictionaryAsync(p => NormalizeScientific(p.Sku));

            foreach (var r in records)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(r.Sku))
                    {
                        result.Skipped++;
                        continue;
                    }

                    await ImportRecordAsync(r, result, existingProducts);
                }
                catch
                {
                    result.Skipped++;
                }
            }

            await _productRepository.SaveChangesAsync();
            return result;
        }

        private async Task ImportRecordAsync(ProductImportDto r, ImportResult result, Dictionary<string, Product> existingProducts)
        {
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
                await _productRepository.SaveChangesAsync();

                existingProducts[sku] = product;
                result.Created++;

                if (!string.IsNullOrWhiteSpace(r.ImageUrl))
                {
                    var urls = r.ImageUrl
                        .Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(u => u.Trim())
                        .ToList();

                    foreach (var (url, index) in urls.Select((value, i) => (value, i)))
                    {
                        var uploaded = await _productImageService.UploadFromUrlAsync(product.Id, url);

                        if (uploaded != null && index == 0)
                        {
                            await _productImageService.SetPrimaryAsync(product.Id, uploaded.Id);
                        }
                    }
                }
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

                _productRepository.Update(existing);
                result.Updated++;

                if (!string.IsNullOrWhiteSpace(r.ImageUrl))
                {
                    var hasImages = await _context.ProductImages.AnyAsync(i => i.ProductId == existing.Id);
                    if (!hasImages)
                    {
                        var urls = r.ImageUrl
                            .Split('|', StringSplitOptions.RemoveEmptyEntries)
                            .Select(u => u.Trim())
                            .ToList();

                        foreach (var (url, index) in urls.Select((value, i) => (value, i)))
                        {
                            var uploaded = await _productImageService.UploadFromUrlAsync(existing.Id, url);

                            if (uploaded != null && index == 0)
                            {
                                await _productImageService.SetPrimaryAsync(existing.Id, uploaded.Id);
                            }
                        }
                    }
                }
            }
        }

        private static int CalculateStock(ProductImportDto record) => record.Available + record.Purchased;

        private static string NormalizeScientific(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";

            value = value.Trim();
            if (value.Contains("E+") || value.Contains("e+"))
            {
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
                    return ((long)number).ToString();
            }
            return value;
        }
    }
}