using DataAccess.Entities;
using ShopShared.Dtos;

namespace ShopAPI.Daos;

public static class ProductDao
{
    public static Product ToEntity(CreateProductDto dto) => new()
    {
        ProductId = Guid.NewGuid(),
        ProductName = dto.ProductName,
        Description = dto.Description,
        Price = dto.Price,
        IsDeleted = false
    };

    public static void ApplyUpdate(Product entity, UpdateProductDto dto)
    {
        entity.ProductName = dto.ProductName;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
    }

    public static ProductResponseDto ToResponseDto(Product entity) => new()
    {
        ProductId = entity.ProductId,
        ProductName = entity.ProductName,
        Description = entity.Description,
        Price = entity.Price,
        IsDeleted = entity.IsDeleted,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
