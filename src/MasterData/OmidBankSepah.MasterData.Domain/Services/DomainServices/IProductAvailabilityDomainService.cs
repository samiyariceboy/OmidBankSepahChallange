using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Domain.Services.DomainServices;

public interface IProductAvailabilityDomainService
{
    Task<ProductInventory> IncreaseProductAvailabilityAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken);

    Task<ProductAvailabilityReservationRes> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<ProductAvailabilityReq> requestedItems,
        CancellationToken cancellationToken);
}
