using CachePOC.Application.Dto;

namespace CachePOC.Application.Repository;

public interface IRepository
{
    Task<List<ProductResponse>> GetProducts();
    Task<ProductResponse?> GetProduct(Guid id);
    Task<ProductResponse> CreateProduct(ProductRequest product);
    Task<ProductResponse?> UpdateProduct(ProductRequest product, Guid id);
    Task<bool> DeleteProduct(Guid id);
}