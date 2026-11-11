using System.Linq.Expressions;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Criterias;

public sealed class CheckProductAvailabilityByProductIdCriteria(Guid productId)
    : CriteriaSpecification<ProductInventory>
{
    public override Expression<Func<ProductInventory, bool>> ToExpression()
    {
        return inventory => inventory.ProductId == productId;
    }
}
