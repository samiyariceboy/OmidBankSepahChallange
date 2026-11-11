using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Domain.Exceptions;

namespace OmidBankSepah.MasterData.Domain.Constraint;

public class MasterDataAssert : Assert
{
    private static readonly MasterDataAssert Instance = new();

    private MasterDataAssert()
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

    public static void NotEmpty<T>(T obj, string name, string? message = null, T? defaultValue = null) where T : class
    {
        ((Assert)Instance).NotEmpty(obj, name, message, defaultValue);
    }

    public static bool GuidIsEmpty(Guid value)
    {
        return ((Assert)Instance).GuidIsEmpty(value);
    }
}
