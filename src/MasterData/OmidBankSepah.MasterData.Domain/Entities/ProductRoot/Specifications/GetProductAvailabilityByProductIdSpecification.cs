using Ardalis.Specification;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Criterias;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Specifications;

public sealed class GetProductAvailabilityByProductIdSpecification
    : BaseSpecification<ProductInventory>
{
    private readonly Guid _productId;

    public GetProductAvailabilityByProductIdSpecification(Guid productId)
    {
        _productId = productId;
        Query.Where(Criteria().ToExpression());
    }

    protected override CriteriaSpecification<ProductInventory> Criteria()
    {
        return new CheckProductAvailabilityByProductIdCriteria(_productId);
    }
}
