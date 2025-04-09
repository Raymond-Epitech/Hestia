using Api.Controllers;
using Business.Interfaces;
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
        var expenseResult = new List<OutputFormatForExpenses>()
        {
            new OutputFormatForExpenses
            {
                Category = "test",
                TotalAmount = 100,
                Expenses = new List<ExpenseOutput>()
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
                        SplitBetween = new Dictionary<Guid, decimal> { { Guid.NewGuid(), 50 }, { Guid.NewGuid(), 30 } },
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
                        SplitBetween = new Dictionary<Guid, decimal> { { Guid.NewGuid(), 50 }, { Guid.NewGuid(), 30 } },
                        DateOfPayment = DateTime.Now,
                    }
                }
            }
        };

        _expenseServiceMock.Setup(service => service.GetAllExpensesAsync(colocationId)).ReturnsAsync(expenseResult);

        // Act
        var actionResult = await _controller.GetAllExpense(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expenseResult);
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
        await Assert.ThrowsAsync<ContextException>(() => _controller.GetAllExpense(colocationId));

        // Assert
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
            SplitBetween = new Dictionary<Guid, decimal> { { Guid.NewGuid(), 50 }, { Guid.NewGuid(), 30 } },
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
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetExpense(expenseId));

        // Assert
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
        await Assert.ThrowsAsync<ContextException>(() => _controller.GetExpense(expenseId));

        // Assert
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

        await Assert.ThrowsAsync<ContextException>(() => _controller.AddExpense(expenseInput));

        _expenseServiceMock.Verify(service => service.AddExpenseAsync(expenseInput), Times.Once);
    }
}
