using OmidBankSepah.MasterData.Domain.Contracts.Responses;

namespace OmidBankSepah.MasterData.Application.Interfaces;

public interface IProductAvailabilityCache
{
    Task<ProductAvailabilityRes?> GetAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task SetAsync(
        ProductAvailabilityRes item,
        TimeSpan lifetime,
        CancellationToken cancellationToken);

    Task RemoveAsync(Guid productId, CancellationToken cancellationToken);
}
