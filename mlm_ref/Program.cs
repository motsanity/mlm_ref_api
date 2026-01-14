using StackExchange.Redis;
using mlm_ref.Infrastructure.Database;
using mlm_ref.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using mlm_ref.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program));

// ---------------------------
// Register your custom services
// ---------------------------

builder.Services.Configure<RabbitMqConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

//get the RabbitMQ configuration
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqConfig>();
Console.WriteLine($"RabbitMQ Host: {rabbitMqConfig.HostName}");

// 1. DbConnectionFactory
builder.Services.AddSingleton<DbConnectionFactory>();

// 2. RabbitMQ publisher
builder.Services.AddSingleton<RabbitMqPublisher>();

// 3. Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetSection("Redis")["Configuration"] ?? "localhost:6379";
    Console.WriteLine($"Connecting to Redis at: {config}");
    return ConnectionMultiplexer.Connect(config);
});

//get the Redis configuration
var redisConfig = builder.Configuration.GetSection("Redis")["Configuration"];
Console.WriteLine($"Redis Configuration: {redisConfig}");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<PlacementService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
