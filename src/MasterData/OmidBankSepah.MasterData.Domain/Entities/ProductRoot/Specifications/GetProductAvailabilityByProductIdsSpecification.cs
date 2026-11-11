using Ardalis.Specification;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Criterias;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Specifications;

public sealed class GetProductAvailabilityByProductIdsSpecification
    : BaseSpecification<ProductInventory>
{
    private readonly Guid[] _productIds;

    public GetProductAvailabilityByProductIdsSpecification(IEnumerable<Guid> productIds)
    {
        _productIds = productIds.Distinct().ToArray();
        Query.Where(Criteria().ToExpression());
    }

    protected override CriteriaSpecification<ProductInventory> Criteria()
    {
        return new CheckProductAvailabilityByProductIdsCriteria(_productIds);
    }
}
