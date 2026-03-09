using ArzotecWebshop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArzotecWebshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly IRackbeatSyncService _syncService;

        public SyncController(IRackbeatSyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> SyncProduct()
        {
            await _syncService.SyncProductsAsync();
            return Ok("Sync started");
        }
    }
}
