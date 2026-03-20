using ArzotecWebshop.Core.DTOs.Import;
using ArzotecWebshop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArzotecWebshop.API.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportsController : ControllerBase
    {
        private readonly IProductImportService _importService;

        public ImportsController(IProductImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("file")]
        public async Task<IActionResult> ImportFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

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
                return BadRequest("Invalid file type. Only use CSV or XML.");
            }

            return Ok(result);
        }
    }
}