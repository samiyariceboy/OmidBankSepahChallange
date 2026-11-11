using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Infrastructure.Context;

namespace OmidBankSepah.MasterData.Infrastructure.Repositories;

public sealed class ProductCategoryRepository(OmidBankSepahMasterDataDbContext dbContext)
    : MasterDataBaseRepository<ProductCategory>(dbContext), IProductCategoryRepository
{
}
