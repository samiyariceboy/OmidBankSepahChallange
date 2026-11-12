using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using OmidBankSepah.MasterData.Application.Interfaces;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;

namespace OmidBankSepah.MasterData.Infrastructure.Services.Cache;

public sealed class RedisCacheService(IDistributedCache distributedCache)
    : IProductAvailabilityCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string KeyPrefix = "masterdata:product-availability:";

    public async Task<ProductAvailabilityRes?> GetAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var value = await distributedCache.GetStringAsync(
            GetKey(productId),
            cancellationToken);

        return value is null
            ? null
            : JsonSerializer.Deserialize<ProductAvailabilityRes>(value, JsonOptions);
    }

    public Task SetAsync(
        ProductAvailabilityRes item,
        TimeSpan lifetime,
        CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime
        };

        return distributedCache.SetStringAsync(
            GetKey(item.ProductId),
            JsonSerializer.Serialize(item, JsonOptions),
            options,
            cancellationToken);
    }

    public Task RemoveAsync(Guid productId, CancellationToken cancellationToken)
    {
        return distributedCache.RemoveAsync(GetKey(productId), cancellationToken);
    }

    private static string GetKey(Guid productId)
        => $"{KeyPrefix}{productId:N}";
}
