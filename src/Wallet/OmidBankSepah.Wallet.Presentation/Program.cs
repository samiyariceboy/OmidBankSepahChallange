using OmidBankSepah.Wallet.Domain.Repositories;
using OmidBankSepah.Order.Presentation.Registeration;
using OmidBankSepah.Wallet.Presentation.Registeration;
using OmidBankSepah.Wallet.Infrastructure.Repositories;
using OmidBankSepah.Wallet.Domain.Entities.WalletRoot.Aggregates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.RegisterDb();
builder.RegisterLoggerServices();
builder.RegisterRepositories();
builder.Services.RegisterMediatR(typeof(WalletTransaction).Assembly);

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
