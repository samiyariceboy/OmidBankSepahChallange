using System.Linq.Expressions;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Criterias;

public sealed class CheckProductAvailabilityByProductIdsCriteria
    : CriteriaSpecification<ProductInventory>
{
    private readonly Guid[] _productIds;

    public CheckProductAvailabilityByProductIdsCriteria(IEnumerable<Guid> productIds)
    {
        _productIds = productIds.Distinct().ToArray();
    }

    public override Expression<Func<ProductInventory, bool>> ToExpression()
    {
        return inventory => _productIds.Contains(inventory.ProductId);
    }
}
