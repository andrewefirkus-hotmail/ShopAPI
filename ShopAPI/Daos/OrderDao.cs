using DataAccess.Entities;
using ShopShared.Dtos;

namespace ShopAPI.Daos;

public static class OrderDao
{
    public static Order ToEntity(CreateOrderDto dto) => new()
    {
        OrderId = Guid.NewGuid(),
        UserId = dto.UserId,
        ProductId = dto.ProductId,
        TotalPaid = dto.TotalPaid,
        IsDeleted = false
    };

    public static void ApplyUpdate(Order entity, UpdateOrderDto dto)
    {
        entity.UserId = dto.UserId;
        entity.ProductId = dto.ProductId;
        entity.TotalPaid = dto.TotalPaid;
    }

    public static OrderResponseDto ToResponseDto(Order entity) => new()
    {
        OrderId = entity.OrderId,
        UserId = entity.UserId,
        ProductId = entity.ProductId,
        TotalPaid = entity.TotalPaid,
        IsDeleted = entity.IsDeleted,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
