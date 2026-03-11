using ArzotecWebshop.Core.DTOs.Import;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IProductImportService
    {
        Task<ImportResult> ImportCsvAsync(Stream filestream);
    }
}