using MediatR;
using Microsoft.EntityFrameworkCore;
using Taday.Corelibrary.Common.Shared;
using OmidBankSepah.MasterData.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Infrastructure.Context;
using OmidBankSepah.MasterData.Presentation.Registeration;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5085, endpointOptions =>
        endpointOptions.Protocols = HttpProtocols.Http2);
});

builder.Services.AddDbContext<OmidBankSepahMasterDataDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sqlOptions => sqlOptions.UseNetTopologySuite()));
builder.Services.AddScoped(
    typeof(IAppLogger<>),
    typeof(Taday.Corelibrary.Common.Shared.Logger<>));
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Products).Assembly));
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductInventoryRepository, ProductInventoryRepository>();
builder.Services.AddScoped<IProductInventoryReservationRepository, ProductInventoryReservationRepository>();
builder.Services.AddMasterDataServices(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<ProductAvailabilityGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
