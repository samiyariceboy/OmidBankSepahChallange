using OmidBankSepah.MasterData.Domain.Constraint;
using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot.Specifications;

namespace OmidBankSepah.MasterData.Domain.Services.DomainServices;
public sealed class ProductAvailabilityDomainService(
    IProductInventoryRepository inventoryRepository,
    IProductInventoryReservationRepository reservationRepository,
    IProductsRepository productsRepository   
) : IProductAvailabilityDomainService
{
    public async Task<ProductInventory> IncreaseProductAvailabilityAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);
        ValidateQuantity(quantity);

        var product = await productsRepository.GetByIdAsync(productId, cancellationToken);
        MasterDataAssert.NotNull(product, nameof(IncreaseProductAvailabilityAsync), MasterDataErrors.ProductNotFound);

        var inventory = await inventoryRepository.FirstOrDefaultAsync(
            new GetProductAvailabilityByProductIdSpecification(productId),
            cancellationToken);

        if (inventory is null)
        {
            inventory = ProductInventory.Create(productId, quantity);
            await inventoryRepository.AddAsync(inventory, saveChanges: true, cancellationToken);
            return inventory;
        }

        inventory.Increase(quantity);
        await inventoryRepository.UpdateAsync(inventory, saveChanges: true, cancellationToken);
        return inventory;
    }

    public async Task<ProductAvailabilityReservationRes> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<ProductAvailabilityReq> requestedItems,
        CancellationToken cancellationToken
    )
    {
        MasterDataAssert.IsSuccess(
            orderId != Guid.Empty,
            [MasterDataErrors.OrderIdInvalid]
        );

        MasterDataAssert.NotNull(
            requestedItems,
            nameof(ReserveProductAvailabilityAsync),
            MasterDataErrors.InventoryItemsRequired
        );

        MasterDataAssert.IsSuccess(
            requestedItems.Count > 0,
            [MasterDataErrors.InventoryItemsRequired]
        );

        var requirements = ConsolidateRequests(requestedItems);

        var existingReservation = await reservationRepository.FirstOrDefaultAsync(
            new GetProductAvailabilityReservationByOrderIdSpecification(orderId),
            cancellationToken
        );

        if (existingReservation is not null)
        {
            MasterDataAssert.IsSuccess(
                existingReservation.Matches(requirements),
                [MasterDataErrors.OrderReservationConflict]);

            return (ProductAvailabilityReservationRes)existingReservation.Clone();
        }

        var inventories = await inventoryRepository.ListAsync(
            new GetProductAvailabilityByProductIdsSpecification(requirements.Select(item => item.ProductId)),
            cancellationToken);

        var inventoryByProductId = inventories.ToDictionary(inventory => inventory.ProductId);

        var reservationItems = requirements
            .Select(item => new ProductAvailabilityReservationItemReq(
                item.ProductId,
                item.Quantity,
                inventoryByProductId.TryGetValue(item.ProductId, out var inventory)
                    ? inventory.Quantity
                    : 0))
            .ToArray();

        var hasUnavailableItems = requirements
            .Any(item =>
                !inventoryByProductId.TryGetValue(item.ProductId, out var inventory) ||
                !inventory.CanDecrease(item.Quantity)
            );

        if (hasUnavailableItems)
        {
            var rejectedReservation = ProductInventoryReservation.Create(
                orderId,
                false,
                reservationItems
            );

            await reservationRepository.AddAsync(
                rejectedReservation,
                saveChanges: true,
                cancellationToken
            );

            return (ProductAvailabilityReservationRes)rejectedReservation.Clone();
        }

        foreach (var requirement in requirements)
        {
            inventoryByProductId[requirement.ProductId].TryDecrease(requirement.Quantity);
        }

        await inventoryRepository.UpdateRangeAsync(
            inventories,
            saveChanges: false,
            cancellationToken
        );

        var reserved = ProductInventoryReservation.Create(orderId, true, reservationItems);
        await reservationRepository.AddAsync(reserved, saveChanges: true, cancellationToken);

        return (ProductAvailabilityReservationRes)reserved.Clone();
    }

    private static IReadOnlyList<ProductAvailabilityReq> ConsolidateRequests(
        IEnumerable<ProductAvailabilityReq> requestedItems
    )
    {
        foreach (var item in requestedItems)
        {
            ValidateProductId(item.ProductId);
            ValidateQuantity(item.Quantity);
        }

        var requirements = requestedItems
            .GroupBy(item => item.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => (long)item.Quantity)
            })
            .ToArray();

        MasterDataAssert.IsSuccess(
            requirements.All(item => item.Quantity <= int.MaxValue),
            [MasterDataErrors.InventoryQuantityInvalid]
        );

        return requirements
            .Select(item => new ProductAvailabilityReq(item.ProductId, (int)item.Quantity))
            .ToArray();
    }

    private static void ValidateProductId(Guid productId)
    {
        MasterDataAssert.IsSuccess(
            productId != Guid.Empty,
            [MasterDataErrors.ProductIdInvalid]
        );
    }

    private static void ValidateQuantity(int quantity)
    {
        MasterDataAssert.IsSuccess(
            quantity > 0,
            [MasterDataErrors.InventoryQuantityInvalid]
        );
    }
}
