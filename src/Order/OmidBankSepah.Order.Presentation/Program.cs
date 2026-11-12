using OmidBankSepah.Order.Presentation.Registeration;
using OmidBankSepah.Order.Presentation.Middlewares;
using OmidBankSepah.Order.Application.UseCases.ProductInventoryChanged;
using OmidBankSepah.Order.Domain.Services.DomainServices;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.RegisterDb();
builder.RegisterLoggerServices();
builder.Services.AddOrderServices(builder.Configuration);
builder.Services.AddOrderRabbitMq(builder.Configuration);
builder.Services.AddScoped<IOrderDomainService, OrderDomainService>();
builder.Services.RegisterMediatR(
    typeof(OrderEntity).Assembly,
    typeof(ProductInventoryChangedCommand).Assembly);

var app = builder.Build();

app.UseMiddleware<OrderExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

app.UseAuthorization();

app.MapControllers();

app.Run();
