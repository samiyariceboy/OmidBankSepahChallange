using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OmidBankSepah.MasterData.Application.Interfaces;
using OmidBankSepah.MasterData.Domain.Contracts.Requests;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Grpc;
using OmidBankSepah.MasterData.Grpc.Services;
using ProductAvailabilityDomainRes = OmidBankSepah.MasterData.Domain.Contracts.Responses.ProductAvailabilityRes;
using ProductAvailabilityReservationDomainRes = OmidBankSepah.MasterData.Domain.Contracts.Responses.ProductAvailabilityReservationRes;

namespace OmidBankSepah.MasterData.IntegrationTests;

public sealed class ProductAvailabilityGrpcIntegrationTests
{
    [Fact]
    public async Task ReserveProductAvailability_UsesGrpcTransportAndPassesAllRequestedItems()
    {
        var applicationService = new RecordingInventoryApplicationService();
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddGrpc();
                services.AddSingleton<IProductAvailabilityApplicationService>(applicationService);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                    endpoints.MapGrpcService<ProductAvailabilityGrpcService>());
            });

        using var server = new TestServer(webHostBuilder);
        using var channel = GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions { HttpHandler = server.CreateHandler() });
        var client = new ProductAvailabilityGrpc.ProductAvailabilityGrpcClient(channel);
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var response = await client.ReserveProductAvailabilityAsync(new ReserveProductAvailabilityReq
        {
            OrderId = orderId.ToString(),
            Items =
            {
                new ProductAvailabilityItemReq
                {
                    ProductId = productId.ToString(),
                    Quantity = 3
                }
            }
        });

        Assert.True(response.IsReserved);
        Assert.Equal(orderId, applicationService.OrderId);
        Assert.Equal(new[] { new ProductAvailabilityReq(productId, 3) }, applicationService.Items);
    }

    private sealed class RecordingInventoryApplicationService : IProductAvailabilityApplicationService
    {
        public Guid? OrderId { get; private set; }

        public IReadOnlyCollection<ProductAvailabilityReq> Items { get; private set; } = [];

        public Task<ProductAvailabilityDomainRes> IncreaseProductAvailabilityAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<ProductAvailabilityReservationDomainRes> ReserveProductAvailabilityAsync(
            Guid orderId,
            IReadOnlyCollection<ProductAvailabilityReq> items,
            CancellationToken cancellationToken)
        {
            OrderId = orderId;
            Items = items;
            return Task.FromResult(new ProductAvailabilityReservationDomainRes(true, []));
        }

        public Task<ProductAvailabilityDomainRes?> GetProductAvailabilityAsync(
            Guid productId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
