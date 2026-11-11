using Ardalis.Specification;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Criterias;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Specifications;

public sealed class GetProductAvailabilityReservationByOrderIdSpecification
    : BaseSpecification<ProductInventoryReservation>
{
    private readonly Guid _orderId;

    public GetProductAvailabilityReservationByOrderIdSpecification(Guid orderId)
    {
        _orderId = orderId;
        Query.Where(Criteria().ToExpression());
        StaticIncludes();
    }

    private void StaticIncludes()
    {
        Query.Include(reservation => reservation.Items);
    }

    protected override CriteriaSpecification<ProductInventoryReservation> Criteria()
    {
        return new CheckProductAvailabilityReservationByOrderIdCriteria(_orderId);
    }
}
