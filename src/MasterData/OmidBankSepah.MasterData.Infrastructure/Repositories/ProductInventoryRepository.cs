using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Infrastructure.Context;

namespace OmidBankSepah.MasterData.Infrastructure.Repositories;

public sealed class ProductInventoryRepository(OmidBankSepahMasterDataDbContext dbContext)
    : MasterDataBaseRepository<ProductInventory>(dbContext), IProductInventoryRepository
{
    public new async Task<ProductInventory> AddAsync(
        ProductInventory entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        dbContext.Set<ProductInventory>().Add(entity);

        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: true,
                cancellationToken);
        }

        return entity;
    }

    public new async Task UpdateAsync(
        ProductInventory entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        dbContext.Set<ProductInventory>().Update(entity);

        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: true,
                cancellationToken);
        }
    }

    public new async Task UpdateRangeAsync(
        IEnumerable<ProductInventory> entities,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        dbContext.Set<ProductInventory>().UpdateRange(entities);

        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: true,
                cancellationToken);
        }
    }
}
