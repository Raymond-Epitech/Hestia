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
    public class ExpenseController(IExpenseService expenseService) : Controller
    {
        [HttpGet("GetByColocationId/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExpenseCategoryOutput>>> GetAllCategoryExpense(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return Ok(await expenseService.GetAllExpenseCategoriesAsync(colocationId));
        }

        [HttpPost("Category/")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddExpenseCategory(ExpenseCategoryInput input)
        {
            if (input.ColocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");
            return Ok(await expenseService.AddExpenseCategoryAsync(input));
        }

        [HttpPut("Category/")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateExpenseCategory(ExpenseCategoryUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Id is required");
            
            return Ok(await expenseService.UpdateExpenseCategoryAsync(input));
        }

        [HttpDelete("Category/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteExpenseCategory(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Id is required");
            return Ok(await expenseService.DeleteExpenseCategoryAsync(id));
        }

        [HttpGet("GetByExpenseCategoryId/{expenseCategoryId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExpenseOutput>>> GetAllExpense(Guid expenseCategoryId)
        {
            if (expenseCategoryId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return Ok(await expenseService.GetAllExpensesAsync(expenseCategoryId));
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

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateExpense(ExpenseUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Id is required");

            return Ok(await expenseService.UpdateExpenseAsync(input));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteExpense(Guid id)
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
        public async Task<ActionResult<Dictionary<Guid, decimal>>> GetBalance(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return Ok(await expenseService.GetAllBalanceAsync(colocationId));
        }

        [HttpGet("GetRefundMethods/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RefundOutput>>> GetRefundMethods(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is required");

            return Ok(await expenseService.GetRefundMethodsAsync(colocationId));
        }
    }
}
