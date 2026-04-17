namespace ShopAPI.Dtos;

public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal TotalPaid { get; set; }
}

public class UpdateOrderDto
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal TotalPaid { get; set; }
}

public class OrderResponseDto
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal TotalPaid { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
