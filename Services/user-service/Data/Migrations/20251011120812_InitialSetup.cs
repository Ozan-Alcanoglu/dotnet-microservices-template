using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace user_service.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 12, 8, 11, 341, DateTimeKind.Utc).AddTicks(9660), new DateTime(2025, 10, 11, 12, 8, 11, 341, DateTimeKind.Utc).AddTicks(9656) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 12, 8, 11, 342, DateTimeKind.Utc).AddTicks(905), new DateTime(2025, 10, 11, 12, 8, 11, 342, DateTimeKind.Utc).AddTicks(904) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 53, 11, 890, DateTimeKind.Utc).AddTicks(5351), new DateTime(2025, 10, 11, 11, 53, 11, 890, DateTimeKind.Utc).AddTicks(5347) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 53, 11, 890, DateTimeKind.Utc).AddTicks(6687), new DateTime(2025, 10, 11, 11, 53, 11, 890, DateTimeKind.Utc).AddTicks(6686) });
        }
    }
}
