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
    public class ReminderController(IReminderService reminderService,
        IPollService pollService,
        IShoppingListService shoppingListService,
        IReactionService reactionService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<ReminderOutput>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReminderOutput>>> GetAllReminders([FromQuery] Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await reminderService.GetAllRemindersAsync(colocationId));
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ReminderOutput), StatusCodes.Status200OK)]
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
        public async Task<ActionResult<Guid>> AddReminder([FromForm] ReminderInput input)
        {
            if (input.ColocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await reminderService.AddReminderAsync(input));
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateReminder([FromBody] ReminderUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await reminderService.UpdateReminderAsync(input));
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

        [HttpGet("images/{fileName}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetImage(string fileName)
        {
            var file = await reminderService.GetImageByNameAsync(fileName);

            return File(file.Content, file.ContentType, file.FileName);
        }

        [HttpDelete("images/{fileName}")]
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

        // Shopping list
        [HttpGet("ShoppingList")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ShoppingItemOutput>>> GetShoppingList([FromQuery] Guid reminderId)
        {
            if (reminderId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await shoppingListService.GetAllShoppingItemsAsync(reminderId));
        }

        [HttpPost("ShoppingList")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddShoppingItem([FromBody] ShoppingItemInput input)
        {
            if (input.ReminderId == Guid.Empty)
                throw new InvalidEntityException("ReminderId is empty");

            return Ok(await shoppingListService.AddShoppingItemAsync(input));
        }

        [HttpPut("ShoppingList")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateShoppingItem([FromBody] ShoppingItemUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await shoppingListService.UpdateShoppingItemAsync(input));
        }

        [HttpDelete("ShoppingList/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteShoppingItem(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await shoppingListService.DeleteShoppingItemAsync(id));
        }

        // PollVotes

        [HttpGet("PollVote")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PollVoteOutput>>> GetAllPollVotes([FromQuery] Guid reminderId)
        {
            if (reminderId == Guid.Empty)
                throw new InvalidEntityException("ReminderId is empty");

            return Ok(await pollService.GetAllPollVoteAsync(reminderId));
        }

        [HttpPost("PollVote")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddPollVote([FromBody] PollVoteInput input)
        {
            if (input.ReminderId == Guid.Empty)
                throw new InvalidEntityException("ReminderId is empty");

            return Ok(await pollService.AddPollVoteAsync(input));
        }

        [HttpDelete("PollVote/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeletePollVote(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await pollService.DeletePollVoteAsync(id));
        }

        // Reactions

        [HttpGet("Reactions")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReactionOutput>>> GetAllReactions([FromQuery] Guid reminderId)
        {
            if (reminderId == Guid.Empty)
                throw new InvalidEntityException("ReminderId is empty");

            return Ok(await reactionService.GetReactionsByPostIdAsync(reminderId));
        }

        [HttpPost("Reactions")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddReaction([FromBody] ReactionInput input)
        {
            if (input.ReminderId == Guid.Empty || input.UserId == Guid.Empty)
                throw new InvalidEntityException("ReminderId is empty");

            return Ok(await reactionService.AddReactionAsync(input));
        }

        [HttpDelete("Reactions")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteReaction([FromBody] ReactionInputForDelete input)
        {
            if (input.UserId == Guid.Empty || input.ReminderId == Guid.Empty)
                throw new InvalidEntityException("Id is empty");

            return Ok(await reactionService.DeleteReactionAsync(input));
        }
    }
}
