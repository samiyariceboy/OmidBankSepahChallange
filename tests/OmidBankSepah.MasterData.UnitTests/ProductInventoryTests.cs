using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Domain.Contracts.Responses;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Contracts.Responses.ProductResponses;
using Taday.Corelibrary.Domain.Exceptions;

namespace OmidBankSepah.MasterData.UnitTests;

public sealed class ProductInventoryTests
{
    [Fact]
    public void TryDecrease_WhenAvailabilityIsSufficient_DecreasesQuantity()
    {
        var inventory = ProductInventory.Create(Guid.NewGuid(), 10);

        var decreased = inventory.TryDecrease(4);

        Assert.True(decreased);
        Assert.Equal(6, inventory.Quantity);
    }

    [Fact]
    public void TryDecrease_WhenAvailabilityIsInsufficient_LeavesQuantityUnchanged()
    {
        var inventory = ProductInventory.Create(Guid.NewGuid(), 3);

        var decreased = inventory.TryDecrease(4);

        Assert.False(decreased);
        Assert.Equal(3, inventory.Quantity);
    }

    [Fact]
    public void Increase_AddsPositiveQuantity()
    {
        var inventory = ProductInventory.Create(Guid.NewGuid(), 3);

        inventory.Increase(4);

        Assert.Equal(7, inventory.Quantity);
    }

    [Fact]
    public void Create_WhenProductIdIsEmpty_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() => ProductInventory.Create(Guid.Empty, 1));
    }

    [Fact]
    public void Create_WhenInitialQuantityIsNegative_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() => ProductInventory.Create(Guid.NewGuid(), -1));
    }

    [Fact]
    public void Clone_ReturnsProductAvailabilityResponse()
    {
        var productId = Guid.NewGuid();
        var inventory = ProductInventory.Create(productId, 8);

        var response = (ProductAvailabilityRes)inventory.Clone();

        Assert.Equal(productId, response.ProductId);
        Assert.Equal(8, response.AvailableQuantity);
    }

    [Fact]
    public void ReservationClone_ReturnsOnlyUnavailableItems()
    {
        var unavailableProductId = Guid.NewGuid();
        var availableProductId = Guid.NewGuid();
        var reservation = ProductInventoryReservation.Create(
            Guid.NewGuid(),
            false,
            [
                new ProductAvailabilityReservationItemReq(unavailableProductId, 5, 2),
                new ProductAvailabilityReservationItemReq(availableProductId, 1, 1)
            ]);

        var response = (ProductAvailabilityReservationRes)reservation.Clone();

        var unavailable = Assert.Single(response.UnavailableProducts);
        Assert.Equal(unavailableProductId, unavailable.ProductId);
        Assert.Equal(5, unavailable.RequestedQuantity);
        Assert.Equal(2, unavailable.AvailableQuantity);
    }

    [Fact]
    public void ProductClone_ReturnsProductResponse()
    {
        var categoryId = Guid.NewGuid();
        var product = Products.Create(
            categoryId,
            " Name ",
            " Brand ",
            " Model ",
            2025,
            " Description ");

        var response = (ProductRes)product.Clone();

        Assert.Equal(product.Id, response.Id);
        Assert.Equal(categoryId, response.ProductCategoryId);
        Assert.Equal("Name", response.ProductName);
        Assert.Equal("Brand", response.BrandName);
        Assert.Equal("Model", response.ModelName);
        Assert.Equal(2025, response.ProductYear);
        Assert.Equal("Description", response.ProductDescription);
    }

    [Fact]
    public void ProductCategoryClone_ReturnsCategoryResponse()
    {
        var category = ProductCategory.Create(
            " Name ",
            " Description ",
            null,
            " CODE ");

        var response = (ProductCategoryRes)category.Clone();

        Assert.Equal(category.Id, response.Id);
        Assert.Equal("Name", response.Name);
        Assert.Equal("Description", response.Description);
        Assert.Equal("CODE", response.Code);
        Assert.Empty(response.Products);
    }
}
