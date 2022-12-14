namespace Application.Products;

public sealed class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public float Weight { get; set; }
}