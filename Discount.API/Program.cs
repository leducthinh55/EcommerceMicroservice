using Discount.API.Extensions;
using Discount.API.Repositories;
using Discount.API.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddHealthChecks()
                .AddNpgSql(configuration["DatabaseSettings:ConnectionString"]);
var app = builder.Build();

// Configure the HTTP request pipeline.s
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase<Program>().Run();
