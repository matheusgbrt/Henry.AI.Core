using Henry.AI.Core.Host.Tokens;
using Mgb.Api.Extensions;
using Mgb.AppRegistration.Extensions;
using Mgb.Consul.Extensions;
using Mgb.DependencyInjections.DependencyInjectons.Extensions;
using Mgb.ServiceBus.ServiceBus.Extensions;
using Microsoft.Extensions.Options;
using Neo4jClient;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterApp("Henry.AI.Core");
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterAllDependencies();
await builder.Configuration.AddConsulConfigurationAsync();
builder.Services.AddConsulRegistration(builder.Configuration);
builder.ConfigureKestrelWithNetworkHelper();
builder.Services.RegisterServiceBus(builder.Configuration);

// Bind options
builder.Services.Configure<Neo4JOptions>(
    builder.Configuration.GetSection("Neo4j"));

// Register a single connected client
builder.Services.AddSingleton<IGraphClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<Neo4JOptions>>().Value;

    var client = new BoltGraphClient(new Uri(opts.Uri), opts.User, opts.Password);

    client.ConnectAsync().GetAwaiter().GetResult();

    return client;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.AddConsulHealthCheck();
app.UseAuthorization();

app.MapControllers();

app.Run();
