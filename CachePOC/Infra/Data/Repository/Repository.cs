using CachePOC.Application.Dto;
using CachePOC.Domain.Entity;
using CachePOC.Infra.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace CachePOC.Application.Repository;

public class Repository(AppDbContext db) : IRepository
{
    public async Task<List<ProductResponse>> GetProducts()
    {
        var products = await db.Products.AsNoTracking().ToListAsync();
        return products.Select(MapperResponse).ToList();
    }

    public async Task<ProductResponse?> GetProduct(Guid id)
    {
        var entity = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null) return null;
        return MapperResponse(entity);
    }

    public async Task<ProductResponse> CreateProduct(ProductRequest request)
    {
        var entity = MapperDomain(request);
        db.Products.Add(entity);
        await db.SaveChangesAsync();
        return MapperResponse(entity);
    }

    public async Task<ProductResponse?> UpdateProduct(ProductRequest request, Guid id)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return null;
        var updatedProduct = MapperUpdate(request, product);
        db.Update(product);
        await db.SaveChangesAsync();
        return MapperResponse(updatedProduct);
    }

    public async Task<bool> DeleteProduct(Guid id)
    {
        var product = await db.Products.FindAsync(id);
        if (product == null) return false;
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }
    
    private Product MapperDomain(ProductRequest product)
    {
        return new Product()
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        };
    }
    private ProductResponse MapperResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        };
    }
    private Product MapperUpdate(ProductRequest product, Product entity)
    {
        entity.Name = product.Name;
        entity.Description = product.Description;
        entity.Price = product.Price;
        entity.Quantity = product.Quantity;
        return entity;
    }
}