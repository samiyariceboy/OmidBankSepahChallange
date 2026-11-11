using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Domain.Exceptions;

namespace OmidBankSepah.Wallet.Domain.Constraint;

public class WalletAssert : Assert
{
    private static readonly WalletAssert Instance = new();

    private WalletAssert()
    {
    }

    protected override void ThrowArgumentNullException(string name, Type type, string? message)
    {
        throw new NotFoundException(message ?? $"{name} is null");
    }

    protected override void ThrowArgumentException(string name, Type type, string? message)
    {
        throw new NotFoundException(message ?? $"{name} is invalid");
    }

    protected override void ThrowOnFailure(string[] message, object? additionalData = null)
    {
        throw new BadRequestException(message, additionalData);
    }

    public static void IsSuccess(bool isSuccess, string[] message, object? additionalData = null)
    {
        ((Assert)Instance).IsSuccess(isSuccess, message, additionalData);
    }

    public static void NotNull<T>(T? obj, string name, string? message = null) where T : class
    {
        ((Assert)Instance).NotNull(obj, name, message);
    }

    public static void NotNull<T>(T? obj, string name, string? message = null) where T : struct
    {
        ((Assert)Instance).NotNull(obj, name, message);
    }
}
