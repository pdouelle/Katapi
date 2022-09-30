namespace Domain.Entities;

public sealed class Bill
{
    public decimal Amount { get; set; }
    public DateTime CreationDate { get; set; }
}