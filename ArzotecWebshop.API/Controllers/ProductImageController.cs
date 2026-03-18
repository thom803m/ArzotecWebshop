using ArzotecWebshop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArzotecWebshop.API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImagesController(IProductImageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int productId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "File is required" });

            try
            {
                using var stream = file.OpenReadStream();

                var image = await _service.UploadImageAsync(productId, stream, file.FileName);

                return Ok(image);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Product with ID {productId} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("{imageId}/primary")]
        public async Task<IActionResult> SetPrimary(int productId, int imageId)
        {
            try
            {
                await _service.SetPrimaryAsync(productId, imageId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Image with ID {imageId} not found for product {productId}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int productId, int imageId)
        {
            try
            {
                await _service.DeleteImageAsync(imageId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Image with ID {imageId} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}