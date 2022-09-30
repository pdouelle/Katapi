namespace Domain.Entities;

public sealed class Order
{
    public string Status { get; set; }
    public ICollection<OrderProduct> OrdersProducts { get; set; }
    public decimal ShipmentAmount { get; set; }
    public decimal TotalAmount { get; set; }
}