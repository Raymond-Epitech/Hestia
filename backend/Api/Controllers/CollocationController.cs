using Business.Interfaces;
using Business.Models.Input;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollocationController(ICollocationService collocationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCollocations()
        {
            try
            {
                var collocations = await collocationService.GetAllCollocations();
                return Ok(collocations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollocation(Guid id)
        {
            try
            {
                var collocation = await collocationService.GetCollocation(id);
                return Ok(collocation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCollocation(CollocationInput collocation)
        {
            try
            {
                await collocationService.AddCollocation(collocation);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCollocation(CollocationUpdate collocation)
        {
            try
            {
                await collocationService.UpdateCollocation(collocation);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollocation(Guid id)
        {
            try
            {
                await collocationService.DeleteCollocation(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
