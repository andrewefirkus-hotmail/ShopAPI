using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ShopDbContext))]
    [Migration("20260416040000_SeedData")]
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO users (user_id, first_name, last_name, address, email, is_deleted) VALUES
                ('11111111-0000-0000-0000-000000000001', 'Alice', 'Smith',  '123 Main St, Springfield',  'alice.smith@example.com',  0),
                ('11111111-0000-0000-0000-000000000002', 'Bob',   'Jones',  '456 Oak Ave, Shelbyville',  'bob.jones@example.com',    0),
                ('11111111-0000-0000-0000-000000000003', 'Carol', 'White',  '789 Pine Rd, Capital City', 'carol.white@example.com',  0);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO products (product_id, product_name, description, price, is_deleted) VALUES
                ('22222222-0000-0000-0000-000000000001', 'Wireless Mouse',      'Ergonomic wireless mouse with USB receiver',                     29.99, 0),
                ('22222222-0000-0000-0000-000000000002', 'Mechanical Keyboard', 'Tenkeyless mechanical keyboard with blue switches',              89.99, 0),
                ('22222222-0000-0000-0000-000000000003', 'USB-C Hub',           '7-in-1 USB-C hub with HDMI, USB 3.0, and SD card reader',       49.99, 0);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO orders (order_id, user_id, product_id, total_paid, is_deleted) VALUES
                ('33333333-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000001', 29.99, 0),
                ('33333333-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000002', 89.99, 0),
                ('33333333-0000-0000-0000-000000000003', '11111111-0000-0000-0000-000000000003', '22222222-0000-0000-0000-000000000003', 49.99, 0);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM orders  WHERE order_id   IN ('33333333-0000-0000-0000-000000000001','33333333-0000-0000-0000-000000000002','33333333-0000-0000-0000-000000000003');
                DELETE FROM products WHERE product_id IN ('22222222-0000-0000-0000-000000000001','22222222-0000-0000-0000-000000000002','22222222-0000-0000-0000-000000000003');
                DELETE FROM users   WHERE user_id     IN ('11111111-0000-0000-0000-000000000001','11111111-0000-0000-0000-000000000002','11111111-0000-0000-0000-000000000003');
            ");
        }
    }
}
