using DataAccess;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Daos;
using ShopAPI.Dtos;

namespace ShopAPI.Services;

public class UserService : IUserService
{
    private readonly ShopDbContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(ShopDbContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _db.Users
            .Where(u => !u.IsDeleted)
            .ToListAsync();

        return users.Select(UserDao.ToResponseDto);
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
        return user is null ? null : UserDao.ToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Check for any existing user with this email, including soft-deleted ones
            var existing = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existing is not null)
            {
                if (!existing.IsDeleted)
                {
                    throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");
                }

                // Reactivate the soft-deleted user with the new details
                UserDao.Reactivate(existing, dto);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Reactivated soft-deleted user {UserId} with email {Email}", existing.UserId, dto.Email);
                return UserDao.ToResponseDto(existing);
            }

            var entity = UserDao.ToEntity(dto);
            _db.Users.Add(entity);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Created user {UserId}", entity.UserId);
            return UserDao.ToResponseDto(entity);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create user");
            throw;
        }
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
            if (entity is null) return null;

            // If the email is being changed, check it isn't taken by another active user
            if (!string.Equals(entity.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var conflict = await _db.Users
                    .AnyAsync(u => u.Email == dto.Email && !u.IsDeleted && u.UserId != id);

                if (conflict)
                    throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");
            }

            UserDao.ApplyUpdate(entity, dto);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Updated user {UserId}", id);
            return UserDao.ToResponseDto(entity);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to update user {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
            if (entity is null) return false;

            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Soft-deleted user {UserId}", id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to delete user {UserId}", id);
            throw;
        }
    }
}
