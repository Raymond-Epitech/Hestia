using Api.Controllers;
using Business.Interfaces;
using EntityFramework.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;

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

    // Get all expenses

    [Fact]
    public async Task GetAllExpense_ReturnsOk_WhenExpenseListIsNotEmpty()
    {
        // Arange
        var colocationId = Guid.NewGuid();
        var expenseList = new List<ExpenseOutput>()
        {
            new ExpenseOutput
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                ColocationId = colocationId,
                Name = "TestExpense1",
                Description = "TestDescription1",
                Amount = 10,
                PaidBy = Guid.NewGuid(),
                SplitBetween = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                DateOfPayment = DateTime.Now,
            },
            new ExpenseOutput
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                ColocationId = colocationId,
                Name = "TestExpense2",
                Description = "TestDescription2",
                Amount = 10,
                PaidBy = Guid.NewGuid(),
                SplitBetween = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                DateOfPayment = DateTime.Now,
            }
        };
        _expenseServiceMock.Setup(service => service.GetAllExpensesAsync(colocationId)).ReturnsAsync(expenseList);

        // Act
        var actionResult = await _controller.GetAllExpense(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expenseList);
        _expenseServiceMock.Verify(service => service.GetAllExpensesAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetAllExpense_ShouldReturnContextError_WhenExpenseListIsInvalid()
    {
        // Arange
        var colocationId = Guid.NewGuid();

        _expenseServiceMock.Setup(service => service.GetAllExpensesAsync(colocationId))
            .ThrowsAsync(new ContextException("Context error"));

        // Act
        var actionResult = await _controller.GetAllExpense(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _expenseServiceMock.Verify(service => service.GetAllExpensesAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetExpense_ReturnsOk_WhenExpenseExists()
    {
        // Arrange
        var expenseId = Guid.NewGuid();
        var expectedExpense = new ExpenseOutput
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            CreatedBy = Guid.NewGuid(),
            ColocationId = Guid.NewGuid(),
            Name = "TestExpense1",
            Description = "TestDescription1",
            Amount = 10,
            PaidBy = Guid.NewGuid(),
            SplitBetween = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            DateOfPayment = DateTime.Now,
        };

        _expenseServiceMock.Setup(service => service.GetExpenseAsync(expenseId)).ReturnsAsync(expectedExpense);

        // Act
        var actionResult = await _controller.GetExpense(expenseId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedExpense);
        _expenseServiceMock.Verify(service => service.GetExpenseAsync(expenseId), Times.Once);
    }

    [Fact]
    public async Task GetExpense_ShouldReturnNotFound_WhenExpenseDoNotExist()
    {
        // Arrange
        var expenseId = Guid.NewGuid();
        _expenseServiceMock.Setup(service => service.GetExpenseAsync(expenseId))
            .ThrowsAsync(new NotFoundException("Expense not found"));

        // Act
        var actionResult = await _controller.GetExpense(expenseId);

        // Assert
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
        _expenseServiceMock.Verify(service => service.GetExpenseAsync(expenseId), Times.Once);
    }

    [Fact]
    public async Task GetExpense_ShouldReturnContextError_WhenExpenseIdIsInvalid()
    {
        // Arrange
        var expenseId = Guid.NewGuid();
        _expenseServiceMock.Setup(service => service.GetExpenseAsync(expenseId))
            .ThrowsAsync(new ContextException("Error in db"));

        // Act
        var actionResult = await _controller.GetExpense(expenseId);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _expenseServiceMock.Verify(service => service.GetExpenseAsync(expenseId), Times.Once);
    }

    [Fact]
    public async Task AddExpense_ReturnsOk_WhenSuccessful()
    {
        var expenseInput = new ExpenseInput
        {
            ColocationId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Name = "TestExpense",
            Description = "TestDescription",
            Amount = 10,
            PaidBy = Guid.NewGuid(),
            SplitType = SplitTypeEnum.Evenly,
            SplitBetween = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            SplitPercentages = null,
            SplitValues = null,
            DateOfPayment = DateTime.Now
        };

        var reminderId = Guid.NewGuid();

        _expenseServiceMock.Setup(service => service.AddExpenseAsync(expenseInput)).ReturnsAsync(reminderId);

        var actionResult = await _controller.AddExpense(expenseInput);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(reminderId);
        _expenseServiceMock.Verify(service => service.AddExpenseAsync(expenseInput), Times.Once);
    }

    [Fact]
    public async Task AddExpense_ShouldReturnInvalidEntity_WhenExpenseInputIsInvalid()
    {
        var expenseInput = new ExpenseInput
        {
            ColocationId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Name = "TestExpense",
            Description = "TestDescription",
            Amount = 10,
            PaidBy = Guid.NewGuid(),
            SplitType = SplitTypeEnum.Evenly,
            SplitBetween = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            SplitPercentages = null,
            SplitValues = null,
            DateOfPayment = DateTime.Now
        };
        _expenseServiceMock.Setup(service => service.AddExpenseAsync(expenseInput))
            .ThrowsAsync(new ContextException("Error in db"));
        var actionResult = await _controller.AddExpense(expenseInput);
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _expenseServiceMock.Verify(service => service.AddExpenseAsync(expenseInput), Times.Once);
    }
}
