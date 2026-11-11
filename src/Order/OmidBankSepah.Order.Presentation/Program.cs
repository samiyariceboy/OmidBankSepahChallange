using DocumentFormat.OpenXml.Drawing.Charts;
using OmidBankSepah.Order.Presentation.Registeration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.RegisterDb();
builder.RegisterLoggerServices();
builder.RegisterRepositories();
builder.Services.RegisterMediatR(typeof(Order).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
