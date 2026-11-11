using System.ComponentModel.DataAnnotations;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using Taday.Corelibrary.Domain.Entities;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

public class ProductInventoryReservationItem : BaseEntity, ICloneable, IValidatableObject
{
    private ProductInventoryReservationItem()
    {
    }

    private ProductInventoryReservationItem(
        ProductInventoryReservation reservation,
        ProductAvailabilityReservationItemReq item)
    {
        ProductInventoryReservation = reservation;
        ProductId = item.ProductId;
        RequestedQuantity = item.RequestedQuantity;
        AvailableQuantity = item.AvailableQuantity;
    }

    public Guid ProductInventoryReservationId { get; private set; }
    public ProductInventoryReservation? ProductInventoryReservation { get; private set; }
    public Guid ProductId { get; private set; }
    public int RequestedQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }

    protected override string[] IntegrityProperties => [];

    internal static ProductInventoryReservationItem Create(
        ProductInventoryReservation reservation,
        ProductAvailabilityReservationItemReq item)
    {
        return new ProductInventoryReservationItem(reservation, item);
    }

    protected override bool SetHasIntegrityChecksum()
    {
        return false;
    }

    public override object GetId()
    {
        return Id;
    }

    public object Clone()
    {
        return new ProductAvailabilityReservationItemRes(
            ProductId,
            RequestedQuantity,
            AvailableQuantity);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}
