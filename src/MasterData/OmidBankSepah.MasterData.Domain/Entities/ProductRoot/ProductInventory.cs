using System.ComponentModel.DataAnnotations;
using OmidBankSepah.MasterData.Domain.Constraint;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

public class ProductInventory : AggregateRoot, ICloneable, IValidatableObject
{
    private ProductInventory()
    {
    }

    private ProductInventory(Guid productId, int quantity)
    {
        Id = ImmediateFillId();
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    protected override string[] IntegrityProperties => [];

    public static ProductInventory Create(Guid productId, int initialQuantity)
    {
        ValidateProductId(productId);
        ValidateQuantity(initialQuantity);

        return new ProductInventory(productId, initialQuantity);
    }

    public void Increase(int quantity)
    {
        ValidateQuantity(quantity);
        MasterDataAssert.IsSuccess(
            quantity <= int.MaxValue - Quantity,
            [MasterDataErrors.InventoryQuantityInvalid]);

        Quantity += quantity;
    }

    public bool CanDecrease(int quantity)
    {
        ValidateQuantity(quantity);
        return Quantity >= quantity;
    }

    public bool TryDecrease(int quantity)
    {
        if (!CanDecrease(quantity))
        {
            return false;
        }

        Quantity -= quantity;
        return true;
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
        return new ProductAvailabilityRes(ProductId, Quantity);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }

    private static void ValidateProductId(Guid productId)
    {
        MasterDataAssert.IsSuccess(
            productId != Guid.Empty,
            [MasterDataErrors.ProductIdInvalid]);
    }

    private static void ValidateQuantity(int quantity)
    {
        MasterDataAssert.IsSuccess(
            quantity > 0,
            [MasterDataErrors.InventoryQuantityInvalid]);
    }
}
