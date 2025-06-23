using System.Text.Json;
using CachePOC.Application.Dto;
using CachePOC.Application.Repository;
using CachePOC.Infra.Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup SQLite Config
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Infra/Data/DataSource/cachePOC.db"));

// Repository DI
builder.Services.AddScoped<IRepository, Repository>();

// Memory Cache Setup
builder.Services.AddMemoryCache();

// Redis Setup
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "redisPOC";
});

var app = builder.Build();

// Endpoints
app.MapGet("/products", async (IRepository repository, IMemoryCache cache, IDistributedCache redis) =>
{
    // Memory
    // if (cache.TryGetValue("products", out var result))
    //     return Results.Ok(result);
    
    // Redis
    var value = await redis.GetStringAsync("products");
    if (value != null)
    {
        var deserialized = JsonSerializer.Deserialize<List<ProductResponse>>(value);
        return Results.Ok(deserialized);
    }
    
    var products = await repository.GetProducts();
    if (!products.Any()) return Results.NotFound("No products found");
    
    // Redis
    var serialized = JsonSerializer.Serialize(products);
    await redis.SetStringAsync("products", serialized, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
    });
    
    // Memory
    // cache.Set("products", products, new MemoryCacheEntryOptions
    // {
    //     AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60),
    //     SlidingExpiration = TimeSpan.FromMinutes(10)
    // });
    
    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (IRepository repository, Guid id, IMemoryCache cache, IDistributedCache redis) =>
{
    var cacheKey = $"products/{id}";
    // Memory
    // if (cache.TryGetValue(cacheKey, out var result))
    //     return Results.Ok(result);
    
    // Redis
    var value = await redis.GetStringAsync(cacheKey);
    if (value != null)
    {
        var deserialized = JsonSerializer.Deserialize<ProductResponse>(value);
        return Results.Ok(deserialized);
    }
    
    var product = await repository.GetProduct(id);
    if (product == null) return Results.NotFound();
    
    // Redis
    var serialized = JsonSerializer.Serialize(product);
    await redis.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
    });
    
    // Memory
    // cache.Set("product", product, new MemoryCacheEntryOptions
    // {
    //     AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60),
    //     SlidingExpiration = TimeSpan.FromMinutes(10)
    // });
    return Results.Ok(product);
});

app.MapPost("/products", async (IRepository repository, ProductRequest product, IDistributedCache redis) =>
{
    var productResponse = await repository.CreateProduct(product);
    // Redis
    var serialized = JsonSerializer.Serialize(productResponse);
    await redis.SetStringAsync(productResponse.Id.ToString(), serialized, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
    });
    return Results.Created($"/products/{productResponse.Id}", productResponse);
});

app.MapPut("/products/{id}", async (IRepository repository, Guid id, ProductRequest product) =>
{
    var productResponse = await repository.GetProduct(id);
    if (productResponse == null) return Results.NotFound();
    return Results.Ok(productResponse);
});

app.MapDelete("/products/{id}", async (IRepository repository, Guid id, IDistributedCache redis) =>
{
    var result = await repository.DeleteProduct(id);
    if (!result) return Results.BadRequest();
    // Redis
    var value =  await redis.GetStringAsync($"products/{id}");
    if (value != null) await redis.RemoveAsync($"products/{id}");
    return Results.NoContent();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();