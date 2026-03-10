using ArzotecWebshop.Core.DTOs;

namespace ArzotecWebshop.Core.Interfaces.Services
{
    public interface IProductImportService
    {
        Task<ImportResult> ImportCsvAsync(Stream filestream);
    }
}