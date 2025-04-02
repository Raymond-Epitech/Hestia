using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController(IExpenseService expenseService) : Controller
    {

        [HttpGet("GetByColocationId/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExpenseOutput>>> GetAllExpense(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return Ok(await expenseService.GetAllExpensesAsync(colocationId));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExpenseOutput>> GetExpense(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is required");

            return Ok(await expenseService.GetExpenseAsync(id));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddExpense(ExpenseInput input)
        {
            if (input.ColocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return Ok(await expenseService.AddExpenseAsync(input));
        }

        [HttpPut("{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateExpense(Guid colocationId, ExpenseUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Id is required");

            await expenseService.UpdateExpenseAsync(input);
            await expenseService.RecalculateBalanceAsync(colocationId);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteExpense(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is required");

            return Ok(await expenseService.DeleteExpenseAsync(id));
        }

        [HttpGet("GetBalance/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BalanceOutput>>> GetBalance(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return await expenseService.GetAllBalanceAsync(colocationId);
        }

        [HttpPut("CalculBalance/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BalanceOutput>>> RecalculBalance(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return await expenseService.RecalculateBalanceAsync(colocationId);
        }
    }
}
