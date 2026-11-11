using System.Linq.Expressions;
using OmidBankSepah.Order.Domain.Entities.OrderRoot;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Criterias;

public sealed class CheckOrderByIdCriteria(Guid orderId) : CriteriaSpecification<Order>
{
    public override Expression<Func<Order, bool>> ToExpression()
    {
        return order => order.Id == orderId;
    }
}
