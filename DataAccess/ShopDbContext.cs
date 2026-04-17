using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).HasColumnName("user_id").HasDefaultValueSql("NEWID()");
            entity.Property(u => u.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(100);
            entity.Property(u => u.Address).HasColumnName("address").HasMaxLength(500);
             entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(256);
                entity.Property(u => u.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(u => u.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("GETUTCDATE()")
                  .ValueGeneratedOnAdd();
            entity.Property(u => u.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("GETUTCDATE()")
                  .ValueGeneratedOnAdd();

            // Filtered unique index: only one active (non-deleted) user per email
            entity.HasIndex(u => u.Email)
                  .IsUnique()
                  .HasFilter("[is_deleted] = 0")
                  .HasDatabaseName("UX_users_email_active");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.ProductId);
            entity.Property(p => p.ProductId).HasColumnName("product_id").HasDefaultValueSql("NEWID()");
            entity.Property(p => p.ProductName).HasColumnName("product_name").IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(p => p.Price).HasColumnName("price").HasColumnType("decimal(18,2)");
            entity.Property(p => p.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("GETUTCDATE()")
                  .ValueGeneratedOnAdd();
            entity.Property(p => p.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("GETUTCDATE()")
                  .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(o => o.OrderId);
            entity.Property(o => o.OrderId).HasColumnName("order_id").HasDefaultValueSql("NEWID()");
            entity.Property(o => o.UserId).HasColumnName("user_id");
            entity.Property(o => o.ProductId).HasColumnName("product_id");
            entity.Property(o => o.TotalPaid).HasColumnName("total_paid").HasColumnType("decimal(18,2)");
            entity.Property(o => o.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(o => o.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(o => o.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(o => o.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("GETUTCDATE()")
                  .ValueGeneratedOnAdd();
            entity.Property(o => o.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("GETUTCDATE()")
                  .ValueGeneratedOnAdd();

            entity.HasOne(o => o.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Product)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(o => o.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
