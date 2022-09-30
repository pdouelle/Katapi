using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }

    [MinLength(3)]
    public string Name { get; set; }

    public decimal Price { get; set; }
    public float Weight { get; set; }
}