using Api.Controllers;
using Business.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia.Controller;

public class ExpenseControllerTests
{
    private readonly Mock<IExpenseService> _expenseServiceMock;
    private readonly ExpenseController _controller;

    public ExpenseControllerTests()
    {
        _expenseServiceMock = new Mock<IExpenseService>();
        _controller = new ExpenseController(_expenseServiceMock.Object);
    }

    [Fact]
    public async Task GetAllCategoryExpense_ReturnsOk_WhenValid()
    {
        var colocationId = Guid.NewGuid();
        var expected = new List<ExpenseCategoryOutput> { new() { Id = Guid.NewGuid(), Name = "Food" } };

        _expenseServiceMock.Setup(s => s.GetAllExpenseCategoriesAsync(colocationId)).ReturnsAsync(expected);

        var result = await _controller.GetAllCategoryExpense(colocationId);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expected);
        _expenseServiceMock.Verify(s => s.GetAllExpenseCategoriesAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task AddExpenseCategory_ReturnsOk_WhenSuccessful()
    {
        var input = new ExpenseCategoryInput { ColocationId = Guid.NewGuid(), Name = "Transport" };
        var expectedId = Guid.NewGuid();

        _expenseServiceMock.Setup(s => s.AddExpenseCategoryAsync(input)).ReturnsAsync(expectedId);

        var result = await _controller.AddExpenseCategory(input);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedId);
        _expenseServiceMock.Verify(s => s.AddExpenseCategoryAsync(input), Times.Once);
    }

    [Fact]
    public async Task UpdateExpenseCategory_ReturnsOk_WhenSuccessful()
    {
        var input = new ExpenseCategoryUpdate { Id = Guid.NewGuid(), Name = "Updated" };

        _expenseServiceMock.Setup(s => s.UpdateExpenseCategoryAsync(input)).ReturnsAsync(input.Id);

        var result = await _controller.UpdateExpenseCategory(input);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(input.Id);
        _expenseServiceMock.Verify(s => s.UpdateExpenseCategoryAsync(input), Times.Once);
    }

    [Fact]
    public async Task DeleteExpenseCategory_ReturnsOk_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        _expenseServiceMock.Setup(s => s.DeleteExpenseCategoryAsync(id)).ReturnsAsync(id);

        var result = await _controller.DeleteExpenseCategory(id);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(id);
        _expenseServiceMock.Verify(s => s.DeleteExpenseCategoryAsync(id), Times.Once);
    }

    [Fact]
    public async Task UpdateExpense_ReturnsOk_WhenSuccessful()
    {
        var input = new ExpenseUpdate { Id = Guid.NewGuid(), Name = "Updated", Amount = 42 };

        _expenseServiceMock.Setup(s => s.UpdateExpenseAsync(input)).ReturnsAsync(input.Id);

        var result = await _controller.UpdateExpense(input);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(input.Id);
        _expenseServiceMock.Verify(s => s.UpdateExpenseAsync(input), Times.Once);
    }

    [Fact]
    public async Task DeleteExpense_ReturnsOk_WhenSuccessful()
    {
        var id = Guid.NewGuid();

        _expenseServiceMock.Setup(s => s.DeleteExpenseAsync(id)).ReturnsAsync(id);

        var result = await _controller.DeleteExpense(id);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(id);
        _expenseServiceMock.Verify(s => s.DeleteExpenseAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetBalance_ReturnsOk_WhenSuccessful()
    {
        var colocationId = Guid.NewGuid();
        var expected = new Dictionary<Guid, decimal> { { Guid.NewGuid(), 50 } };

        _expenseServiceMock.Setup(s => s.GetAllBalanceAsync(colocationId)).ReturnsAsync(expected);

        var result = await _controller.GetBalance(colocationId);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expected);
        _expenseServiceMock.Verify(s => s.GetAllBalanceAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetRefundMethods_ReturnsOk_WhenSuccessful()
    {
        var colocationId = Guid.NewGuid();
        var expected = new List<RefundOutput> { new() { From = Guid.NewGuid(), To = Guid.NewGuid(), Amount = 20 } };

        _expenseServiceMock.Setup(s => s.GetRefundMethodsAsync(colocationId)).ReturnsAsync(expected);

        var result = await _controller.GetRefundMethods(colocationId);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expected);
        _expenseServiceMock.Verify(s => s.GetRefundMethodsAsync(colocationId), Times.Once);
    }
}
