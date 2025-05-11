using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Output;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppingListController(IShoppingListService shoppingListService) : Controller
{
    [HttpGet("GetByColocationId/{colocationId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShoppingListName>>> GetAllShoppingListName(Guid colocationId)
    {
        if (colocationId == Guid.Empty)
            return BadRequest("ColocationId is empty");

        return Ok(await shoppingListService.GetAllShoppingListNameAsync(colocationId));
    }
}

