using Api.Controllers;
using Business.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia.Controller;

public class ShoppingListControllerTests
{
    private readonly Mock<IShoppingListService> _shoppingListServiceMock;
    private readonly ShoppingListController _controller;

    public ShoppingListControllerTests()
    {
        _shoppingListServiceMock = new Mock<IShoppingListService>();
        _controller = new ShoppingListController(_shoppingListServiceMock.Object);
    }

    [Fact]
    public async Task GetAllShoppingListName_ShouldReturnOk_WhenValid()
    {
        var colocationId = Guid.NewGuid();
        var expected = new List<ShoppingListName> { new() { Id = Guid.NewGuid(), Name = "Groceries" } };

        _shoppingListServiceMock.Setup(s => s.GetAllShoppingListNameAsync(colocationId)).ReturnsAsync(expected);

        var result = await _controller.GetAllShoppingListName(colocationId);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expected);
        _shoppingListServiceMock.Verify(s => s.GetAllShoppingListNameAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetShoppingList_ShouldReturnOk_WhenFound()
    {
        var id = Guid.NewGuid();
        var expected = new ShoppingListOutput { Id = id };

        _shoppingListServiceMock.Setup(s => s.GetShoppingListAsync(id)).ReturnsAsync(expected);

        var result = await _controller.GetShoppingList(id);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expected);
        _shoppingListServiceMock.Verify(s => s.GetShoppingListAsync(id), Times.Once);
    }

    [Fact]
    public async Task AddShoppingList_ShouldReturnOk_WhenValid()
    {
        var input = new ShoppingListInput { ColocationId = Guid.NewGuid(), Name = "List" };
        var expected = Guid.NewGuid();

        _shoppingListServiceMock.Setup(s => s.AddShoppingList(input)).ReturnsAsync(expected);

        var result = await _controller.AddShoppingList(input);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expected);
        _shoppingListServiceMock.Verify(s => s.AddShoppingList(input), Times.Once);
    }

    [Fact]
    public async Task AddShoppingItem_ShouldReturnOk_WhenValid()
    {
        var input = new ShoppingItemInput { ShoppingListId = Guid.NewGuid(), Name = "Milk" };
        var expected = Guid.NewGuid();

        _shoppingListServiceMock.Setup(s => s.AddShoppingItem(input)).ReturnsAsync(expected);

        var result = await _controller.AddShoppingItem(input);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(expected);
        _shoppingListServiceMock.Verify(s => s.AddShoppingItem(input), Times.Once);
    }

    [Fact]
    public async Task UpdateShoppingList_ShouldReturnOk_WhenUpdated()
    {
        var input = new ShoppingListUpdate { Id = Guid.NewGuid(), Name = "Updated" };

        _shoppingListServiceMock.Setup(s => s.UpdateShoppingList(input)).ReturnsAsync(input.Id);

        var result = await _controller.UpdateShoppingList(input);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(input.Id);
        _shoppingListServiceMock.Verify(s => s.UpdateShoppingList(input), Times.Once);
    }

    [Fact]
    public async Task DeleteShoppingList_ShouldReturnOk_WhenDeleted()
    {
        var id = Guid.NewGuid();

        _shoppingListServiceMock.Setup(s => s.DeleteShoppingList(id)).ReturnsAsync(id);

        var result = await _controller.DeleteShoppingList(id);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(id);
        _shoppingListServiceMock.Verify(s => s.DeleteShoppingList(id), Times.Once);
    }

    [Fact]
    public async Task DeleteShoppingItem_ShouldReturnOk_WhenDeleted()
    {
        var id = Guid.NewGuid();

        _shoppingListServiceMock.Setup(s => s.DeleteShoppingItem(id)).ReturnsAsync(id);

        var result = await _controller.DeleteShoppingItem(id);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(id);
        _shoppingListServiceMock.Verify(s => s.DeleteShoppingItem(id), Times.Once);
    }
}
