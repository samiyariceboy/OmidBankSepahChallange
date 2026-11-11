using System.Linq.Expressions;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Criterias;

public sealed class CheckProductAvailabilityReservationByOrderIdCriteria(Guid orderId)
    : CriteriaSpecification<ProductInventoryReservation>
{
    public override Expression<Func<ProductInventoryReservation, bool>> ToExpression()
    {
        return reservation => reservation.OrderId == orderId;
    }
}
