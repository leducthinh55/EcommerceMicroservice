using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;
builder.Services.AddOcelot();
var hostBuilder = Host.CreateDefaultBuilder(args);

builder.Configuration.AddJsonFile("ocelot.Development.json", true, true);

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
await app.UseOcelot();
app.Run();
