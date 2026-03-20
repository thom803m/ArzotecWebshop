using ArzotecWebshop.Core.DTOs.Brands;
using ArzotecWebshop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArzotecWebshop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandService.GetBrandsAsync();
                
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);

            if (brand == null)
                return NotFound();

            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDto dto)
        {
            var brand = await _brandService.CreateBrandAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBrandDto dto)
        {
            var brand = await _brandService.UpdateBrandAsync(id, dto);

            if (brand == null)
                return NotFound();

            return Ok(brand);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteBrandAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
