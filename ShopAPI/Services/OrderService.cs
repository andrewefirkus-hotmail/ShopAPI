using DataAccess;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Daos;
using ShopShared.Dtos;

namespace ShopAPI.Services;

public class OrderService : IOrderService
{
    private readonly ShopDbContext _db;
    private readonly ILogger<OrderService> _logger;

    public OrderService(ShopDbContext db, ILogger<OrderService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _db.Orders
            .Where(o => !o.IsDeleted)
            .ToListAsync();

        return orders.Select(OrderDao.ToResponseDto);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id && !o.IsDeleted);
        return order is null ? null : OrderDao.ToResponseDto(order);
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = OrderDao.ToEntity(dto);
            _db.Orders.Add(entity);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Created order {OrderId}", entity.OrderId);
            return OrderDao.ToResponseDto(entity);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create order");
            throw;
        }
    }

    public async Task<OrderResponseDto?> UpdateAsync(Guid id, UpdateOrderDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id && !o.IsDeleted);
            if (entity is null) return null;

            OrderDao.ApplyUpdate(entity, dto);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Updated order {OrderId}", id);
            return OrderDao.ToResponseDto(entity);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to update order {OrderId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id && !o.IsDeleted);
            if (entity is null) return false;

            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Soft-deleted order {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to delete order {OrderId}", id);
            throw;
        }
    }
}
