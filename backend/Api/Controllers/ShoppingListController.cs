using Business.Interfaces;
using EntityFramework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

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
            throw new InvalidEntityException("Colocation id is empty");

        return Ok(await shoppingListService.GetAllShoppingListNameAsync(colocationId));
    }

    [HttpGet("GetById/{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> GetShoppingList(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvalidEntityException("Id is empty");

        return Ok(await shoppingListService.GetShoppingListAsync(id));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> AddShoppingList(ShoppingListInput shoppingListInput)
    {
        if (shoppingListInput.ColocationId == Guid.Empty)
            throw new InvalidEntityException("colocation id is empty");

        return Ok(await shoppingListService.AddShoppingList(shoppingListInput));
    }

    [HttpPost("Item/")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> AddShoppingItem(ShoppingItemInput shoppingItemInput)
    {
        if (shoppingItemInput.ShoppingListId == Guid.Empty)
            throw new InvalidEntityException("shopping list id is empty");

        return Ok(await shoppingListService.AddShoppingItem(shoppingItemInput));
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> UpdateShoppingList(ShoppingListUpdate shoppingListInput)
    {
        if (shoppingListInput.Id == Guid.Empty)
            throw new InvalidEntityException("id is empty");

        return Ok(await shoppingListService.UpdateShoppingList(shoppingListInput));
    }

    [HttpPut("Item/")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> UpdateShoppingItem(ShoppingItemUpdate shoppingItemInput)
    {
        if (shoppingItemInput.ShoppingListId == Guid.Empty)
            throw new InvalidEntityException("shopping list id is empty");

        return Ok(await shoppingListService.UpdateShoppingItem(shoppingItemInput));
    }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> DeleteShoppingList(Guid shoppingListId)
    {
        if (shoppingListId == Guid.Empty)
            throw new InvalidEntityException("id is empty");

        return Ok(await shoppingListService.DeleteShoppingList(shoppingListId));
    }

    [HttpDelete("Item/")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingListOutput>> DeleteShoppingItem(Guid shoppingItemId)
    {
        if (shoppingItemId == Guid.Empty)
            throw new InvalidEntityException("shopping list id is empty");

        return Ok(await shoppingListService.DeleteShoppingItem(shoppingItemId));
    }
}

