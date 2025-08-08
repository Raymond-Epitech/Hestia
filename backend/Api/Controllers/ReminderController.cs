using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController(IReminderService reminderService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetByColocation/{colocationId}")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReminderOutput>>> GetAllReminders(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await reminderService.GetAllRemindersAsync(colocationId));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReminderOutput>> GetReminder(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await reminderService.GetReminderAsync(id));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddReminder(ReminderInput input)
        {
            if (input.ColocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            if (input.IsImage && (input.Image is null || input.Image.Length == 0))
                throw new InvalidEntityException("Image is empty or invalid");

            if (!input.IsImage && input.Content.IsNullOrEmpty())
                throw new InvalidEntityException("Content is empty or invalid");

            return Ok(await reminderService.AddReminderAsync(input));
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateReminder(ReminderUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await reminderService.UpdateReminderAsync(input));
        }

        [HttpPut("Range")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UpdateRangeReminder(List<ReminderUpdate> inputs)
        {
            if (inputs.Any(x => x.Id == Guid.Empty))
                throw new InvalidEntityException("Id is empty");

            return Ok(await reminderService.UpdateRangeReminderAsync(inputs));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteReminder(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await reminderService.DeleteReminderAsync(id));
        }

        [HttpGet("/images/{fileName}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetImage(string fileName)
        {
            var file = await reminderService.GetImageByNameAsync(fileName);

            return File(file.Content, file.ContentType, file.FileName);
        }

        [HttpDelete("/images/{fileName}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new InvalidEntityException("File name is empty");

            return Ok(reminderService.DeleteImage(fileName));
        }
    }
}
