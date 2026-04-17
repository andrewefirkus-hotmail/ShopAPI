using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccess;

public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
{
    public ShopDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=ShopDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
        return new ShopDbContext(optionsBuilder.Options);
    }
}
