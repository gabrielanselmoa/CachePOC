namespace CachePOC.Application.Dto;

public record ProductRequest( string Name, string? Description, decimal Price, int Quantity);

public record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}