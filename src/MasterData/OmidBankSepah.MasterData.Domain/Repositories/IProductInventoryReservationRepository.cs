using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Domain.Repositories;

public interface IProductInventoryReservationRepository
    : IMasterDataBaseRepository<ProductInventoryReservation>
{
    new Task<ProductInventoryReservation> AddAsync(
        ProductInventoryReservation entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
