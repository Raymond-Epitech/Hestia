using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Update;
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
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
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
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
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
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
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
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
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
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
