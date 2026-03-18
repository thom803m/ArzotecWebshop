using ArzotecWebshop.Core.DTOs.Import;
using ArzotecWebshop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArzotecWebshop.API.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly IProductImportService _importService;

        public ImportController(IProductImportService importService)
        {
            _importService = importService;
        }

        // Unified import endpoint (CSV eller XML)
        [HttpPost("file")]
        public async Task<IActionResult> ImportFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Ingen fil blev uploadet.");

            var fileName = file.FileName.ToLowerInvariant();
            ImportResult result;

            if (fileName.EndsWith(".csv"))
            {
                result = await _importService.ImportCsvAsync(file.OpenReadStream());
            }
            else if (fileName.EndsWith(".xml"))
            {
                result = await _importService.ImportXmlAsync(file.OpenReadStream());
            }
            else
            {
                return BadRequest("Ugyldig filtype. Kun CSV eller XML understøttes.");
            }

            return Ok(result);
        }
    }
}