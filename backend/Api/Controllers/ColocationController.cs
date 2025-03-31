using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColocationController(IColocationService colocationService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ColocationOutput>>> GetAllCollocations()
        {
            return Ok(await colocationService.GetAllColocations());
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ColocationOutput>> GetColocation(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("id is empty");

            return Ok(await colocationService.GetColocation(id));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddCollocation(ColocationInput colocation, Guid AddedBy)
        {
            return Ok(await colocationService.AddColocation(colocation, AddedBy));
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateCollocation(ColocationUpdate colocation)
        {
            if (colocation.Id == Guid.Empty)
                throw new InvalidEntityException("id is empty");

            return Ok(await colocationService.UpdateColocation(colocation));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteCollocation(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("id is empty");

            return Ok(await colocationService.DeleteColocation(id));
        }
    }
}
