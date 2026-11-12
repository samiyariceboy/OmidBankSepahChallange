using OmidBankSepah.Order.Presentation.Registeration;
using OmidBankSepah.MasterData.Presentation.Middlewares;
using OmidBankSepah.MasterData.Presentation.Registeration;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;
using OmidBankSepah.MasterData.Application.UseCases.ProductInventoryChanged;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.RegisterDb();
builder.RegisterLoggerServices();
builder.RegisterRepositories();
builder.Services.AddMasterDataServices(builder.Configuration);
builder.Services.AddMasterDataRabbitMq(builder.Configuration);
builder.Services.RegisterMediatR(
    typeof(ProductCategory).Assembly,
    typeof(ProductInventoryChangedCommand).Assembly);

var app = builder.Build();

app.UseMiddleware<MasterDataExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
