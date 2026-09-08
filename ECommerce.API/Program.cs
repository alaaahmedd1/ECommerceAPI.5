using ECommerce.API.Middleware;
using ECommerce.Application.DependencyInjection;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.DependencyInjection;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Services;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.Services;
using StackExchange.Redis;
using ECommerce.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// MemoryCache
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<AppHub>("/chatHub");

app.Run();


app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


// Cache Service
builder.Services.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));
public partial class Program { }
