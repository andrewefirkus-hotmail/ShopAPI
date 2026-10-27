using DataAccess;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Daos;
using ShopShared.Dtos;

namespace ShopAPI.Services;

public class ProductService : IProductService
{
    private readonly ShopDbContext _db;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ShopDbContext db, ILogger<ProductService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _db.Products
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        return products.Select(ProductDao.ToResponseDto);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
        return product is null ? null : ProductDao.ToResponseDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = ProductDao.ToEntity(dto);
            _db.Products.Add(entity);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Created product {ProductId}", entity.ProductId);
            return ProductDao.ToResponseDto(entity);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create product");
            throw;
        }
    }

    public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
            if (entity is null) return null;

            ProductDao.ApplyUpdate(entity, dto);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Updated product {ProductId}", id);
            return ProductDao.ToResponseDto(entity);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to update product {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
            if (entity is null) return false;

            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Soft-deleted product {ProductId}", id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to delete product {ProductId}", id);
            throw;
        }
    }
}
