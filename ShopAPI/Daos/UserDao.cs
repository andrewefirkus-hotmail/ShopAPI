using DataAccess.Entities;
using ShopAPI.Dtos;

namespace ShopAPI.Daos;

public static class UserDao
{
    public static User ToEntity(CreateUserDto dto) => new()
    {
        UserId = Guid.NewGuid(),
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Address = dto.Address,
        Email = dto.Email,
        IsDeleted = false
    };

    public static void ApplyUpdate(User entity, UpdateUserDto dto)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Address = dto.Address;
        entity.Email = dto.Email;
    }

    public static void Reactivate(User entity, CreateUserDto dto)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Address = dto.Address;
        entity.Email = dto.Email;
        entity.IsDeleted = false;
    }

    public static UserResponseDto ToResponseDto(User entity) => new()
    {
        UserId = entity.UserId,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Address = entity.Address,
        Email = entity.Email,
        IsDeleted = entity.IsDeleted,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
