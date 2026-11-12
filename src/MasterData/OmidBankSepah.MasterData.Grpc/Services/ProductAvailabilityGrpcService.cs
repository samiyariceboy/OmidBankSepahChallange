using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using OmidBankSepah.MasterData.Application.Interfaces;
using OmidBankSepah.MasterData.Domain.Constraint;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using Taday.Corelibrary.Domain.Exceptions;

namespace OmidBankSepah.MasterData.Grpc.Services;

public sealed class ProductAvailabilityGrpcService(
    IProductAvailabilityApplicationService productAvailabilityApplicationService)
    : ProductAvailabilityGrpc.ProductAvailabilityGrpcBase
{
    public override async Task<ProductAvailabilityRes> IncreaseProductAvailability(
        IncreaseProductAvailabilityReq request,
        ServerCallContext context)
    {
        var productId = ParseGuid(request.ProductId, "ProductId");
        var availability = await ExecuteWithErrorMapping(
            () => productAvailabilityApplicationService.IncreaseProductAvailabilityAsync(
                productId,
                request.Quantity,
                context.CancellationToken));

        return new ProductAvailabilityRes
        {
            Found = true,
            ProductId = availability.ProductId.ToString(),
            AvailableQuantity = availability.AvailableQuantity
        };
    }

    public override async Task<ProductAvailabilityRes> GetProductAvailability(
        GetProductAvailabilityReq request,
        ServerCallContext context)
    {
        var productId = ParseGuid(request.ProductId, "ProductId");
        var availability = await ExecuteWithErrorMapping(
            () => productAvailabilityApplicationService.GetProductAvailabilityAsync(
                productId,
                context.CancellationToken));

        return availability is null
            ? new ProductAvailabilityRes { Found = false, ProductId = productId.ToString() }
            : new ProductAvailabilityRes
            {
                Found = true,
                ProductId = availability.ProductId.ToString(),
                AvailableQuantity = availability.AvailableQuantity
            };
    }

    public override async Task<ProductAvailabilityReservationRes> ReserveProductAvailability(
        ReserveProductAvailabilityReq request,
        ServerCallContext context)
    {
        var orderId = ParseGuid(request.OrderId, "OrderId");
        var items = request.Items
            .Select(item => new ProductAvailabilityReq(
                ParseGuid(item.ProductId, "ProductId"),
                item.Quantity))
            .ToArray();

        var result = await ExecuteWithErrorMapping(
            () => productAvailabilityApplicationService.ReserveProductAvailabilityAsync(
                orderId,
                items,
                context.CancellationToken));

        var response = new ProductAvailabilityReservationRes { IsReserved = result.IsReserved };
        response.UnavailableItems.AddRange(result.UnavailableProducts.Select(item =>
            new UnavailableProductAvailabilityRes
            {
                ProductId = item.ProductId.ToString(),
                RequestedQuantity = item.RequestedQuantity,
                AvailableQuantity = item.AvailableQuantity
            }));

        return response;
    }

    private static Guid ParseGuid(string value, string fieldName)
    {
        if (!Guid.TryParse(value, out var parsed) || parsed == Guid.Empty)
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                $"{fieldName} must be a non-empty GUID."));
        }

        return parsed;
    }

    private static async Task<T> ExecuteWithErrorMapping<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new RpcException(new Status(
                StatusCode.Aborted,
                "Product availability changed concurrently. Retry the request."));
        }
        catch (BadRequestException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (NotFoundException exception)
        {
            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
    }
}
