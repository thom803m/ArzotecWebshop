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

        [HttpPost("csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            var result = await _importService.ImportCsvAsync(file.OpenReadStream());

            return Ok(result);
        }
    }
}
