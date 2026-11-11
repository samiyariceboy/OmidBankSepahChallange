using OmidBankSepah.Order.Presentation.Registeration;
using OmidBankSepah.MasterData.Presentation.Registeration;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.RegisterDb();
builder.RegisterLoggerServices();
builder.RegisterRepositories();
builder.Services.RegisterMediatR(typeof(ProductCategory).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
