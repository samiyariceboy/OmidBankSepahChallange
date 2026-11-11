using Ardalis.Specification;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Criterias;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Specifications;

public sealed class GetOrderByIdempotencyKeySpecification : BaseSpecification<Order>
{
    private readonly string _idempotencyKey;

    public GetOrderByIdempotencyKeySpecification(string idempotencyKey)
    {
        _idempotencyKey = idempotencyKey;
        Query.Where(Criteria().ToExpression());
        StaticIncludes();
    }

    private void StaticIncludes()
    {
        Query.Include(order => order.Items);
    }

    protected override CriteriaSpecification<Order> Criteria()
    {
        return new CheckOrderByIdempotencyKeyCriteria(_idempotencyKey);
    }
}
