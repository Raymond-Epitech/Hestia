using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController(IExpenseService expenseService) : Controller
    {

        [HttpGet("GetByColocationId/{ColocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExpenseOutput>>> GetAllExpense(Guid ColocationId)
        {
            try
            {
                var expenses = await expenseService.GetAllExpensesAsync(ColocationId);
                return Ok(expenses);
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

        [HttpGet("GetById/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExpenseOutput>> GetExpense(Guid id)
        {
            try
            {
                return Ok(await expenseService.GetExpenseAsync(id));
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddExpense(ExpenseInput input)
        {
            try
            {
                var id = await expenseService.AddExpenseAsync(input);
                return Ok(id);
            }
            catch (ContextException ex)
            {
                return StatusCode(500, ex);
            }
            catch (InvalidEntityException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // Update expense

        // Remove expense

        [HttpGet("GetBalance/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BalanceOutput>>> GetBalance(Guid colocationId)
        {
            try
            {
                return await expenseService.GetAllBalanceAsync(colocationId);
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

        [HttpPut("CalculBalance/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BalanceOutput>>> RecalculBalance(Guid colocationId)
        {
            try
            {
                return await expenseService.RecalculateBalanceAsync(colocationId);
            }
            catch (ContextException ex)
            {
                return StatusCode(500, ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
