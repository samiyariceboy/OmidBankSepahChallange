using System.Security.Cryptography;
using System.Text;
using OmidBankSepah.Order.Application.Contracts.Requests;
using OmidBankSepah.Order.Application.Contracts.Responses;
using OmidBankSepah.Order.Domain.Constraint;
using OmidBankSepah.Order.Domain.Contracts.Requests;
using OmidBankSepah.Order.Domain.Contracts.Responses;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Specifications;
using OmidBankSepah.Order.Domain.Repositories;
using OmidBankSepah.Order.Domain.Services.DomainServices;
using RedLockNet;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Application.Services.Orders;

public sealed class OrderApplicationService(
    IDistributedLockFactory lockFactory,
    IOrderRepository orderRepository,
    IOrderDomainService orderDomainService,
    IProductAvailabilityGrpcProxy productAvailabilityGrpcProxy) : IOrderApplicationService
{
    private static readonly TimeSpan LockExpiry = TimeSpan.FromMinutes(2);

    public async Task<OrderRes> CreateAsync(
        CreateOrderReq request,
        CancellationToken cancellationToken)
    {
        OrderAssert.IsSuccess(
            request is not null,
            [OrderErrors.OrderRequestRequired]);
        OrderAssert.IsSuccess(
            !string.IsNullOrWhiteSpace(request.IdempotencyKey),
            [OrderErrors.IdempotencyKeyRequired]);
        OrderAssert.IsSuccess(
            request.Items is not null,
            [OrderErrors.OrderItemsRequired]);

        var drafts = request.Items
            .Select(item => new OrderItemReq(item.ProductId, item.Price, item.Quantity))
            .ToArray();
        var preparedOrder = orderDomainService.CreatePendingOrder(
            request.CustomerId,
            request.IdempotencyKey,
            drafts);

        await using var distributedLock = await lockFactory.CreateLockAsync(
            GetOrderLockKey(preparedOrder.IdempotencyKey),
            LockExpiry,
            TimeSpan.Zero,
            TimeSpan.Zero,
            cancellationToken);

        OrderAssert.IsSuccess(
            distributedLock.IsAcquired,
            [OrderErrors.OrderLockUnavailable]);

        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByIdempotencyKeySpecification(preparedOrder.IdempotencyKey),
            cancellationToken);

        if (order is not null)
        {
            OrderAssert.IsSuccess(
                MatchesRequest(order, request, drafts),
                [OrderErrors.OrderIdempotencyConflict]);

            if (order.Status != OmidBankSepah.Order.Domain.Enums.OrderStatus.Pending)
            {
                return (OrderRes)order.Clone();
            }
        }
        else
        {
            order = preparedOrder;
            await orderRepository.AddAsync(
                order,
                saveChanges: true,
                cancellationToken);
        }

        var reservation = await productAvailabilityGrpcProxy.ReserveProductAvailabilityAsync(
            order.Id,
            order.Items.Select(item => (item.ProductId, item.Quantity)).ToArray(),
            cancellationToken);

        if (reservation.IsReserved)
        {
            order.Confirm();
        }
        else
        {
            var reason = "Insufficient product availability for: "
                + string.Join(
                    ", ",
                    reservation.UnavailableItems.Select(item =>
                        $"{item.ProductId} (requested {item.RequestedQuantity}, available {item.AvailableQuantity})"));
            order.Reject(reason);
        }

        await orderRepository.UpdateAsync(
            order,
            saveChanges: true,
            cancellationToken);

        return (OrderRes)order.Clone();
    }

    public async Task<OrderRes?> GetByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        if (orderId == Guid.Empty)
        {
            return null;
        }

        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByIdSpecification(orderId),
            cancellationToken);

        return order is null ? null : (OrderRes)order.Clone();
    }

    private static bool MatchesRequest(
        OrderEntity order,
        CreateOrderReq request,
        IReadOnlyCollection<OrderItemReq> drafts)
    {
        return order.MatchesRequest(
            request.CustomerId,
            drafts.Select(item => (item.ProductId, item.Price, item.Quantity)));
    }

    private static string GetOrderLockKey(string idempotencyKey)
    {
        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(idempotencyKey)));
        return $"order:create:{hash}";
    }
}
