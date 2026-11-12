using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;

namespace OmidBankSepah.MasterData.Application.Interfaces;

public interface IProductAvailabilityApplicationService
{
    Task<ProductAvailabilityRes> IncreaseProductAvailabilityAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken);

    Task<ProductAvailabilityReservationRes> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<ProductAvailabilityReq> items,
        CancellationToken cancellationToken);

    Task<ProductAvailabilityRes?> GetProductAvailabilityAsync(
        Guid productId,
        CancellationToken cancellationToken);
}
