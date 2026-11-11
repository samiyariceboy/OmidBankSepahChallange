using OmidBankSepah.Order.Domain.Contracts.Requests;
using OmidBankSepah.Order.Domain.Contracts.Responses;
using OmidBankSepah.Order.Domain.Enums;
using OmidBankSepah.Order.Domain.Services.DomainServices;
using Taday.Corelibrary.Domain.Exceptions;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.UnitTests;

public sealed class OrderDomainServiceTests
{
    private readonly OrderDomainService _service = new();

    [Fact]
    public void CreatePendingOrder_WhenItemsAreEmpty_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() =>
            _service.CreatePendingOrder(Guid.NewGuid(), "request-1", []));
    }

    [Fact]
    public void CreatePendingOrder_WhenQuantityIsNotPositive_ThrowsBadRequestException()
    {
        var items = new[] { new OrderItemReq(Guid.NewGuid(), 5m, 0) };

        Assert.Throws<BadRequestException>(() =>
            _service.CreatePendingOrder(Guid.NewGuid(), "request-1", items));
    }

    [Fact]
    public void CreatePendingOrder_WhenProductIdIsEmpty_ThrowsBadRequestException()
    {
        var items = new[] { new OrderItemReq(Guid.Empty, 5m, 1) };

        Assert.Throws<BadRequestException>(() =>
            _service.CreatePendingOrder(Guid.NewGuid(), "request-1", items));
    }

    [Fact]
    public void CreatePendingOrder_WithValidItems_CalculatesTotalAndStartsPending()
    {
        var items = new[]
        {
            new OrderItemReq(Guid.NewGuid(), 5m, 2),
            new OrderItemReq(Guid.NewGuid(), 3m, 4)
        };

        var order = _service.CreatePendingOrder(Guid.NewGuid(), "request-1", items);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(22m, order.TotalAmount);
        Assert.Equal(2, order.Items.Count);
    }

    [Fact]
    public void Confirm_WhenOrderIsPending_ConfirmsIt()
    {
        var order = CreateOrderWithOneItem();

        order.Confirm();

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Reject_WhenOrderIsPending_StoresReason()
    {
        var order = CreateOrderWithOneItem();

        order.Reject("Insufficient product availability.");

        Assert.Equal(OrderStatus.Rejected, order.Status);
        Assert.Equal("Insufficient product availability.", order.RejectionReason);
    }

    [Fact]
    public void Clone_ReturnsOrderResponseWithClonedItems()
    {
        var productId = Guid.NewGuid();
        var order = _service.CreatePendingOrder(
            Guid.NewGuid(),
            "request-1",
            [new OrderItemReq(productId, 5m, 2)]);

        var response = (OrderRes)order.Clone();

        Assert.Equal(order.Id, response.Id);
        Assert.Equal(10m, response.TotalAmount);
        var item = Assert.Single(response.Items);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(5m, item.Price);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(10m, item.TotalPrice);
    }

    private OrderEntity CreateOrderWithOneItem()
    {
        return _service.CreatePendingOrder(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            [new OrderItemReq(Guid.NewGuid(), 5m, 1)]);
    }
}
