using System.Globalization;
using System.Resources;

namespace OmidBankSepah.MasterData.Domain.Constraint;

public static class MasterDataErrors
{
    private static readonly ResourceManager Resources = new(
        "OmidBankSepah.MasterData.Domain.Constraint.MasterDataErrors",
        typeof(MasterDataErrors).Assembly);

    public static string ProductCategoryIdInvalid => Get(nameof(ProductCategoryIdInvalid));
    public static string ProductNameRequired => Get(nameof(ProductNameRequired));
    public static string BrandNameRequired => Get(nameof(BrandNameRequired));
    public static string ModelNameRequired => Get(nameof(ModelNameRequired));
    public static string ProductYearInvalid => Get(nameof(ProductYearInvalid));
    public static string ProductDescriptionRequired => Get(nameof(ProductDescriptionRequired));
    public static string CategoryNameRequired => Get(nameof(CategoryNameRequired));
    public static string CategoryDescriptionRequired => Get(nameof(CategoryDescriptionRequired));
    public static string ParentCategoryIdInvalid => Get(nameof(ParentCategoryIdInvalid));
    public static string CategoryCodeRequired => Get(nameof(CategoryCodeRequired));
    public static string ProductIdInvalid => Get(nameof(ProductIdInvalid));
    public static string ProductNotFound => Get(nameof(ProductNotFound));
    public static string InventoryQuantityInvalid => Get(nameof(InventoryQuantityInvalid));
    public static string InventoryItemsRequired => Get(nameof(InventoryItemsRequired));
    public static string OrderIdInvalid => Get(nameof(OrderIdInvalid));
    public static string OrderReservationConflict => Get(nameof(OrderReservationConflict));
    public static string InventoryLockUnavailable => Get(nameof(InventoryLockUnavailable));

    private static string Get(string key)
    {
        return Resources.GetString(key, CultureInfo.CurrentUICulture)
            ?? throw new MissingManifestResourceException($"Resource key '{key}' was not found.");
    }
}
