using Ardalis.Specification;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Criterias;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Specifications;

public sealed class GetOrderByIdSpecification : BaseSpecification<Order>
{
    private readonly Guid _orderId;

    public GetOrderByIdSpecification(Guid orderId)
    {
        _orderId = orderId;
        Query.Where(Criteria().ToExpression());
        StaticIncludes();
    }

    private void StaticIncludes()
    {
        Query.Include(order => order.Items);
    }

    protected override CriteriaSpecification<Order> Criteria()
    {
        return new CheckOrderByIdCriteria(_orderId);
    }
}
