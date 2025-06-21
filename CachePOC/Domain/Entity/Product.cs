namespace CachePOC.Domain.Entity;

public class Product
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}