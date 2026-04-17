using DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ShopAPI.Services;
using Swashbuckle.AspNetCore.Swagger;

//purpose of the project is a mess with entity framework and swagger,
//I wanted to use ef migrations for code-first setup.
//because this is built with ai agents (copilot and claude code) things can move quickly to test architecture ideas.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console()
        .WriteTo.File("logs/shopapi-.log", rollingInterval: RollingInterval.Day));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("BlazorWasm", policy =>
        {
            // TODO: In production, replace AllowAnyOrigin with the specific WASM origin (e.g. https://yourapp.azurestaticapps.net)
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "ShopAPI",
            Version = "v1"
        });
    });

    // DefaultConnection: restricted runtime account (SELECT/INSERT/UPDATE/DELETE only)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    // MigrationConnection: privileged account with DDL rights, used only at startup for migrations
    var migrationConnectionString = builder.Configuration.GetConnectionString("MigrationConnection")
        ?? throw new InvalidOperationException("Connection string 'MigrationConnection' not found.");

    builder.Services.AddDbContext<ShopDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var migrationOptions = new DbContextOptionsBuilder<ShopDbContext>()
                .UseSqlServer(migrationConnectionString)
                .Options;

            using var migrationDb = new ShopDbContext(migrationOptions);
            migrationDb.Database.Migrate();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Database migration failed; continuing without applying migrations.");
        }
    }

    // Show detailed errors in development so we can see why swagger generation fails (500)
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // Try to pre-generate the Swagger document at startup to catch errors early and log them
    try
    {
        var swaggerProvider = app.Services.GetService(typeof(ISwaggerProvider)) as ISwaggerProvider;
        if (swaggerProvider is not null)
        {
            var doc = swaggerProvider.GetSwagger("v1");
            Log.Information("Swagger document generated successfully (pre-warm).");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to generate Swagger document at startup. See exception for details.");
    }

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopAPI v1"));

    app.UseCors("BlazorWasm");
    app.UseSerilogRequestLogging();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
