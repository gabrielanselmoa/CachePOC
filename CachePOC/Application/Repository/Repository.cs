using CachePOC.Application.Dto;
using CachePOC.Domain.Entity;
using CachePOC.Infra.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace CachePOC.Application.Repository;

public class Repository (AppDbContext db) : IRepository
{
    public async Task<List<ProductResponse>> GetProducts()
    {
        var products = await db.Products.AsNoTracking().ToListAsync();
        var result = products.Select(p => new ProductResponse
        {
            Id =  p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
        }).ToList();
        return result;
    }

    public async Task<ProductResponse?> GetProduct(Guid id)
    {
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return null;
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        };
    }

    public async Task<ProductResponse> CreateProduct(ProductRequest request)
    {
        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
        };
        
        db.Products.Add(entity);
        await db.SaveChangesAsync();
        
        return new ProductResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Quantity = entity.Quantity,
        };
    }

    public async Task<ProductResponse?> UpdateProduct(ProductRequest request, Guid id)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return null;
        
        var updatedProduct = Mapper(request, product);
        
        db.Update(product);
        await db.SaveChangesAsync();

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
        };
    }

    public async Task<bool> DeleteProduct(Guid id)
    {
        var product = await db.Products.FindAsync(id);
        if (product == null) return false;
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }

    private Product Mapper(ProductRequest product, Product entity)
    {
        entity.Name = product.Name;
        entity.Description = product.Description;
        entity.Price = product.Price;
        entity.Quantity = product.Quantity;
        return entity;
    }
}