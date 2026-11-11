using OmidBankSepah.Order.Application.Contracts.Responses;

namespace OmidBankSepah.Order.Application.Services.Orders;

public interface IProductAvailabilityGrpcProxy
{
    Task<ProductAvailabilityReservationRes> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<(Guid ProductId, int Quantity)> items,
        CancellationToken cancellationToken);
}
