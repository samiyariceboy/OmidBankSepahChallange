using System.Resources;
using System.Globalization;

namespace OmidBankSepah.Order.Domain.Constraint;
public static class OrderErrors
{
    private static readonly ResourceManager Resources = new(
        "OmidBankSepah.Order.Domain.Constraint.OrderErrors",
        typeof(OrderErrors).Assembly);

    public static string CustomerIdInvalid => Get(nameof(CustomerIdInvalid));
    public static string OrderStatusInvalid => Get(nameof(OrderStatusInvalid));
    public static string OrderItemIdInvalid => Get(nameof(OrderItemIdInvalid));
    public static string OrderItemNotFound => Get(nameof(OrderItemNotFound));
    public static string OrderRequired => Get(nameof(OrderRequired));
    public static string OrderItemAlreadyAttached => Get(nameof(OrderItemAlreadyAttached));
    public static string QuantityMustBePositive => Get(nameof(QuantityMustBePositive));
    public static string PriceCannotBeNegative => Get(nameof(PriceCannotBeNegative));
    public static string ProductIdInvalid => Get(nameof(ProductIdInvalid));
    public static string OrderItemsRequired => Get(nameof(OrderItemsRequired));
    public static string IdempotencyKeyRequired => Get(nameof(IdempotencyKeyRequired));
    public static string IdempotencyKeyTooLong => Get(nameof(IdempotencyKeyTooLong));
    public static string OrderIdempotencyConflict => Get(nameof(OrderIdempotencyConflict));
    public static string OrderMustBePending => Get(nameof(OrderMustBePending));
    public static string OrderLockUnavailable => Get(nameof(OrderLockUnavailable));
    public static string OrderRequestRequired => Get(nameof(OrderRequestRequired));

    private static string Get(string key)
    {
        return Resources.GetString(key, CultureInfo.CurrentUICulture)
            ?? throw new MissingManifestResourceException($"Resource key '{key}' was not found.");
    }
}
