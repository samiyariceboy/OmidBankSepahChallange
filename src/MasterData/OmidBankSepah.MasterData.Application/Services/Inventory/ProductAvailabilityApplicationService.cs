using RedLockNet;
using OmidBankSepah.MasterData.Domain.Constraint;
using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.IntegrationEvents.ProductInventory;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Domain.Services.DomainServices;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Specifications;
using OmidBankSepah.MasterData.Application.Interfaces;

namespace OmidBankSepah.MasterData.Application.Services.Inventory;
public sealed class ProductAvailabilityApplicationService(
    IDistributedLockFactory lockFactory,
    IProductAvailabilityDomainService inventoryDomainService,
    IProductInventoryRepository inventoryRepository,
    IProductAvailabilityCache inventoryCache,
    IProductInventoryEventPublisher eventPublisher) : IProductAvailabilityApplicationService
{
    private static readonly TimeSpan LockExpiry = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan LockWait = TimeSpan.Zero;
    private static readonly TimeSpan LockRetry = TimeSpan.Zero;
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromSeconds(15);

    public async Task<ProductAvailabilityRes> IncreaseProductAvailabilityAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        MasterDataAssert.IsSuccess(
            productId != Guid.Empty,
            [MasterDataErrors.ProductIdInvalid]);

        MasterDataAssert.IsSuccess(
            quantity > 0,
            [MasterDataErrors.InventoryQuantityInvalid]);

        await using var productLock = await AcquireLockAsync(
            GetProductLockKey(productId),
            cancellationToken);

        await inventoryCache.RemoveAsync(productId, cancellationToken);
        var inventory = await inventoryDomainService.IncreaseProductAvailabilityAsync(
            productId,
            quantity,
            cancellationToken);
        var response = (ProductAvailabilityRes)inventory.Clone();

        await eventPublisher.TryPublishAsync(
            new ProductInventoryChangedMessage(
                Guid.NewGuid(),
                null,
                "ProductAvailabilityIncreased",
                [new ProductInventoryChangedItem(productId, quantity)],
                DateTimeOffset.UtcNow),
            cancellationToken);

        return response;
    }

    public async Task<ProductAvailabilityReservationRes> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<ProductAvailabilityReq> items,
        CancellationToken cancellationToken)
    {
        MasterDataAssert.NotNull(
            items,
            nameof(ReserveProductAvailabilityAsync),
            MasterDataErrors.InventoryItemsRequired
        );

        await using var orderLock = await AcquireLockAsync(
            $"masterdata:inventory:order:{orderId:N}",
            cancellationToken);

        var productLocks = new List<IRedLock>();
        try
        {
            foreach (var productId in items
                         .Select(item => item.ProductId)
                         .Distinct()
                         .Order())
            {
                productLocks.Add(await AcquireLockAsync(
                    GetProductLockKey(productId),
                    cancellationToken));
            }

            foreach (var productId in items.Select(item => item.ProductId).Distinct())
            {
                await inventoryCache.RemoveAsync(productId, cancellationToken);
            }

            var reservation = await inventoryDomainService.ReserveProductAvailabilityAsync(
                orderId,
                items,
                cancellationToken);

            if (reservation.IsReserved)
            {
                await eventPublisher.TryPublishAsync(
                    new ProductInventoryChangedMessage(
                        orderId,
                        orderId,
                        "ProductAvailabilityReserved",
                        items
                            .GroupBy(item => item.ProductId)
                            .Select(group => new ProductInventoryChangedItem(
                                group.Key,
                                // نکته مهم برای خبر به کم شدن موجودی محصول اشاره دارد، بنابراین مقدار منفی است
                                -group.Sum(item => item.Quantity)))

                            .ToArray(),
                        DateTimeOffset.UtcNow),
                    cancellationToken);
            }

            return reservation;
        }
        finally
        {
            for (var index = productLocks.Count - 1; index >= 0; index--)
            {
                await productLocks[index].DisposeAsync();
            }
        }
    }

    public async Task<ProductAvailabilityRes?> GetProductAvailabilityAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await using var productLock = await AcquireLockAsync(
            GetProductLockKey(productId),
            cancellationToken);

        var cached = await inventoryCache.GetAsync(productId, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var inventory = await inventoryRepository.FirstOrDefaultAsync(
            new GetProductAvailabilityByProductIdSpecification(productId),
            cancellationToken);

        if (inventory is null)
        {
            return null;
        }

        var availability = (ProductAvailabilityRes)inventory.Clone();
        await inventoryCache.SetAsync(availability, CacheLifetime, cancellationToken);
        return availability;
    }

    private async Task<IRedLock> AcquireLockAsync(
        string resource,
        CancellationToken cancellationToken)
    {
        var distributedLock = await lockFactory.CreateLockAsync(
            resource,
            LockExpiry,
            LockWait,
            LockRetry,
            cancellationToken);

        if (!distributedLock.IsAcquired)
        {
            await distributedLock.DisposeAsync();

            MasterDataAssert.IsSuccess(
                false,
                [MasterDataErrors.InventoryLockUnavailable]
            );
        }

        return distributedLock;
    }

    private static string GetProductLockKey(Guid productId)
        => $"masterdata:inventory:product:{productId:N}";
}
