using OmidBankSepah.Order.Application.Contracts.Responses;

namespace OmidBankSepah.Order.Application.Interfaces;

public interface IProductAvailabilityGrpcProxy
{
    Task<ProductAvailabilityReservationRes> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<(Guid ProductId, int Quantity)> items,
        CancellationToken cancellationToken);
}
