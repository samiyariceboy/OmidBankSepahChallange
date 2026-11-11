using OmidBankSepah.Order.Presentation.Registeration;
using OmidBankSepah.Order.Presentation.Middlewares;
using OmidBankSepah.Order.Domain.Services.DomainServices;
using Scalar.AspNetCore;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.RegisterDb();
builder.RegisterLoggerServices();
builder.Services.AddOrderServices(builder.Configuration);
builder.Services.AddScoped<IOrderDomainService, OrderDomainService>();
builder.Services.RegisterMediatR(typeof(OrderEntity).Assembly);

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
