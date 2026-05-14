using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Formica.ApiService.Warehouse.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseFoundationInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "warehouse_foundation");

            migrationBuilder.CreateTable(
                name: "location_address_rules",
                schema: "warehouse_foundation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MaxLength = table.Column<int>(type: "integer", nullable: false),
                    AllowedPattern = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NormalizeToUppercase = table.Column<bool>(type: "boolean", nullable: false),
                    TrimWhitespace = table.Column<bool>(type: "boolean", nullable: false),
                    ZonePrefixRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_address_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                schema: "warehouse_foundation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_location_address_rules_Code",
                schema: "warehouse_foundation",
                table: "location_address_rules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_Code",
                schema: "warehouse_foundation",
                table: "warehouses",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location_address_rules",
                schema: "warehouse_foundation");

            migrationBuilder.DropTable(
                name: "warehouses",
                schema: "warehouse_foundation");
        }
    }
}
