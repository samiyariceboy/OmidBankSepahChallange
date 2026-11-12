using MediatR;
using OmidBankSepah.Identity.Application.UseCases.ProductInventoryChanged;
using OmidBankSepah.Identity.Presentation.Registeration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(ProductInventoryChangedCommand).Assembly));
builder.Services.AddIdentityRabbitMq(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
