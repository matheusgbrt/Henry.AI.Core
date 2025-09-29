using Henry.AI.Core.Host.Tokens;
using Mgb.DependencyInjections.DependencyInjectons.Extensions;
using Mgb.ServiceBus.ServiceBus.Extensions;
using Microsoft.Extensions.Options;
using Neo4jClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterAllDependencies();
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
app.UseAuthorization();

app.MapControllers();

app.Run();
