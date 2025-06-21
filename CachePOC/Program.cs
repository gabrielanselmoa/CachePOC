using CachePOC.Application.Dto;
using CachePOC.Application.Repository;
using CachePOC.Infra.Data.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup SQLite Config
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Infra/Data/DataSource/cachePOC.db"));

// Repository DI
builder.Services.AddScoped<IRepository, Repository>();

var app = builder.Build();

// Endpoints
app.MapGet("/products", async (IRepository repository) =>
{
    var products = await repository.GetProducts();
    if (!products.Any()) return Results.NotFound("No products found");
    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (IRepository repository, Guid id) =>
{
    var product = await repository.GetProduct(id);
    if (product == null) return Results.NotFound();
    return Results.Ok(product);
});

app.MapPost("/products", async (IRepository repository, ProductRequest product) =>
{ 
    var productResponse = await repository.CreateProduct(product);
    return Results.Created($"/products/{productResponse.Id}", productResponse);
});

app.MapPut("/products/{id}", async (IRepository repository, Guid id, ProductRequest product) =>
{
    var productResponse = await repository.GetProduct(id);
    if (productResponse == null) return Results.NotFound();
    return Results.Ok(productResponse);
});

app.MapDelete("/products/{id}", async (IRepository repository, Guid id) =>
{
    var result = await repository.DeleteProduct(id);
    if (!result) return Results.BadRequest();
    return Results.NoContent();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();