using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Infrastructure.Context;

namespace OmidBankSepah.MasterData.Infrastructure.Repositories;

public sealed class ProductInventoryReservationRepository(
    OmidBankSepahMasterDataDbContext dbContext)
    : MasterDataBaseRepository<ProductInventoryReservation>(dbContext),
        IProductInventoryReservationRepository
{
    public new async Task<ProductInventoryReservation> AddAsync(
        ProductInventoryReservation entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        dbContext.Set<ProductInventoryReservation>().Add(entity);

        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: true,
                cancellationToken);
        }

        return entity;
    }
}
