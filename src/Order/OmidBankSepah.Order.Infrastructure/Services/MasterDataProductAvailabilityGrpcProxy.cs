using Grpc.Core;
using Microsoft.Extensions.Logging;
using OmidBankSepah.MasterData.Grpc;
using OmidBankSepah.Order.Application.Services.Orders;
using ProductAvailabilityReservationReply = OmidBankSepah.Order.Application.Contracts.Responses.ProductAvailabilityReservationRes;
using UnavailableProductAvailabilityReply = OmidBankSepah.Order.Application.Contracts.Responses.UnavailableProductAvailabilityRes;

namespace OmidBankSepah.Order.Infrastructure.Services;

public sealed class MasterDataProductAvailabilityGrpcProxy(
    ProductAvailabilityGrpc.ProductAvailabilityGrpcClient client,
    ILogger<MasterDataProductAvailabilityGrpcProxy> logger) : IProductAvailabilityGrpcProxy
{
    public async Task<ProductAvailabilityReservationReply> ReserveProductAvailabilityAsync(
        Guid orderId,
        IReadOnlyCollection<(Guid ProductId, int Quantity)> items,
        CancellationToken cancellationToken)
    {
        var request = new ReserveProductAvailabilityReq { OrderId = orderId.ToString() };
        request.Items.AddRange(items.Select(item => new ProductAvailabilityItemReq
        {
            ProductId = item.ProductId.ToString(),
            Quantity = item.Quantity
        }));

        try
        {
            var response = await client.ReserveProductAvailabilityAsync(
                request,
                cancellationToken: cancellationToken);

            return new ProductAvailabilityReservationReply(
                response.IsReserved,
                response.UnavailableItems.Select(item =>
                {
                    if (!Guid.TryParse(item.ProductId, out var productId))
                    {
                        throw new InvalidDataException(
                            "MasterData returned an invalid product ID in its inventory response.");
                    }

                    return new UnavailableProductAvailabilityReply(
                        productId,
                        item.RequestedQuantity,
                        item.AvailableQuantity);
                }).ToArray());
        }
        catch (RpcException exception)
        {
            logger.LogError(
                exception,
                "Product availability gRPC call failed for order {OrderId}. Status: {StatusCode}",
                orderId,
                exception.StatusCode);
            throw;
        }
    }
}
