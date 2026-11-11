using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Domain.Repositories;

public interface IProductInventoryRepository : IMasterDataBaseRepository<ProductInventory>
{
    new Task<ProductInventory> AddAsync(
        ProductInventory entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    new Task UpdateAsync(
        ProductInventory entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    new Task UpdateRangeAsync(
        IEnumerable<ProductInventory> entities,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
