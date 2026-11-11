using System.Linq.Expressions;
using OmidBankSepah.Order.Domain.Entities.OrderRoot;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Criterias;

public sealed class CheckOrderByIdempotencyKeyCriteria(string idempotencyKey)
    : CriteriaSpecification<Order>
{
    public override Expression<Func<Order, bool>> ToExpression()
    {
        return order => order.IdempotencyKey == idempotencyKey;
    }
}
