using System.Globalization;
using System.Resources;

namespace OmidBankSepah.Wallet.Domain.Constraint;

internal static class WalletErrors
{
    private static readonly ResourceManager Resources = new(
        "OmidBankSepah.Wallet.Domain.Constraint.WalletErrors",
        typeof(WalletErrors).Assembly);

    public static string CustomerIdInvalid => Get(nameof(CustomerIdInvalid));
    public static string WalletRequired => Get(nameof(WalletRequired));
    public static string TransactionRequired => Get(nameof(TransactionRequired));
    public static string TransactionAlreadyAttached => Get(nameof(TransactionAlreadyAttached));
    public static string TransactionNotSuccessful => Get(nameof(TransactionNotSuccessful));
    public static string IdempotencyKeyTooLong => Get(nameof(IdempotencyKeyTooLong));

    private static string Get(string key)
    {
        return Resources.GetString(key, CultureInfo.CurrentUICulture)
            ?? throw new MissingManifestResourceException($"Resource key '{key}' was not found.");
    }
}
