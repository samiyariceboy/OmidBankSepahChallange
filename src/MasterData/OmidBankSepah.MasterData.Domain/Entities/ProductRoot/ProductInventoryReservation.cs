using System.ComponentModel.DataAnnotations;
using OmidBankSepah.MasterData.Domain.Constraint;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

public class ProductInventoryReservation : AggregateRoot, ICloneable, IValidatableObject
{
    private readonly List<ProductInventoryReservationItem> _items = [];

    private ProductInventoryReservation()
    {
    }

    private ProductInventoryReservation(
        Guid orderId,
        bool isReserved,
        IEnumerable<ProductAvailabilityReservationItemReq> items)
    {
        Id = ImmediateFillId();
        OrderId = orderId;
        IsReserved = isReserved;
        foreach (var item in items)
        {
            _items.Add(ProductInventoryReservationItem.Create(this, item));
        }
    }

    public Guid OrderId { get; private set; }
    public bool IsReserved { get; private set; }
    public IReadOnlyList<ProductInventoryReservationItem> Items => _items.AsReadOnly();

    protected override string[] IntegrityProperties => [];

    public static ProductInventoryReservation Create(
        Guid orderId,
        bool isReserved,
        IEnumerable<ProductAvailabilityReservationItemReq> items)
    {
        MasterDataAssert.IsSuccess(
            orderId != Guid.Empty,
            [MasterDataErrors.OrderIdInvalid]);

        return new ProductInventoryReservation(orderId, isReserved, items);
    }

    public object Clone()
    {
        var unavailable = IsReserved
            ? Array.Empty<ProductAvailabilityReservationItemRes>()
            : _items
                .Select(item => (ProductAvailabilityReservationItemRes)item.Clone())
                .Where(item => item.AvailableQuantity < item.RequestedQuantity)
                .ToArray();

        return new ProductAvailabilityReservationRes(IsReserved, unavailable);
    }

    public bool Matches(IEnumerable<ProductAvailabilityReq> requestedItems)
    {
        var requested = requestedItems
            .OrderBy(item => item.ProductId)
            .Select(item => (item.ProductId, item.Quantity))
            .ToArray();
        var reserved = _items
            .OrderBy(item => item.ProductId)
            .Select(item => (item.ProductId, item.RequestedQuantity))
            .ToArray();

        return requested.SequenceEqual(reserved);
    }

    protected override bool SetHasIntegrityChecksum()
    {
        return false;
    }

    public override object GetId()
    {
        return Id;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}
