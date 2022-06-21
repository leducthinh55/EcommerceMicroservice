using AutoMapper;
using Basket.API.GrpcServices;
using Basket.API.Mapper;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Discount.Grpc.Protos;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = configuration.GetValue<string>("CacheStrings:ConnectionString");
});
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration.GetValue<string>("EventBusSettings:HostAddress"));
        cfg.ConfigureEndpoints(ctx);
    });
});
builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
{
    // if specified, waits until the bus is started before
    // returning from IHostedService.StartAsync
    // default is false
    options.WaitUntilStarted = true;

    // if specified, limits the wait time when starting the bus
    options.StartTimeout = TimeSpan.FromSeconds(10);

    // if specified, limits the wait time when stopping the bus
    options.StopTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new BasketProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
    o => o.Address = new Uri(configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
