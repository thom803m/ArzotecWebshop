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

        // Only here for testing by typing the URL for the image
        [HttpPost("url")]
        public async Task<IActionResult> UploadFromUrl(int productId, [FromBody] string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return BadRequest(new { Message = "Image URL is required" });

            try
            {
                var image = await _service.UploadFromUrlAsync(productId, imageUrl);

                if (image == null)
                    return BadRequest(new { Message = "Failed to add image from URL" });

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