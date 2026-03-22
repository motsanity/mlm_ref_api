using StackExchange.Redis;
using mlm_ref.Infrastructure.Database;
using mlm_ref.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using mlm_ref.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ADD CORS SERVICE ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program));

// Register custom services
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetSection("Redis")["Configuration"] ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddScoped<PlacementService>();

var app = builder.Build();

// --- 2. CONFIGURE PIPELINE ORDER ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT: UseCors must be AFTER UseRouting and BEFORE UseAuthorization/Endpoints
app.UseRouting();

// This allows your HTML/JS file to talk to the API
app.UseCors("AllowAll"); 

// If testing locally with plain HTML, you might want to comment this out 
// if your browser complains about SSL certificates
// app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();