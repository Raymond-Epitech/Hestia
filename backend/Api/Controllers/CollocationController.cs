using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollocationController(ICollocationService collocationService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAllCollocations()
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
        [AllowAnonymous]
        public async Task<ActionResult<CollocationOutput>> GetCollocation(Guid id)
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
        [Authorize]
        public async Task<ActionResult<Guid>> AddCollocation(CollocationInput collocation, Guid? AddedBy)
        {
            try
            {
                var collocId = await collocationService.AddCollocation(collocation, AddedBy);
                return Ok(collocId);
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
        [Authorize]
        public async Task<ActionResult> UpdateCollocation(CollocationUpdate collocation)
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
        [Authorize]
        public async Task<ActionResult> DeleteCollocation(Guid id)
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
