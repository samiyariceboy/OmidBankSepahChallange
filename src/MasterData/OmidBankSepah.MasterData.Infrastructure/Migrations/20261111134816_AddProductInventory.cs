using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmidBankSepah.MasterData.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductInventoryId",
                table: "EntityVersions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductInventoryReservationId",
                table: "EntityVersions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductInventoryReservationItemId",
                table: "EntityVersions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    HasIntegrityChecksum = table.Column<bool>(type: "bit", nullable: false),
                    IntegrityChecksum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductInventoryReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsReserved = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    HasIntegrityChecksum = table.Column<bool>(type: "bit", nullable: false),
                    IntegrityChecksum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventoryReservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductInventoryReservationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ProductInventoryReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedQuantity = table.Column<int>(type: "int", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    HasIntegrityChecksum = table.Column<bool>(type: "bit", nullable: false),
                    IntegrityChecksum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventoryReservationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInventoryReservationItems_ProductInventoryReservations_ProductInventoryReservationId",
                        column: x => x.ProductInventoryReservationId,
                        principalTable: "ProductInventoryReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityVersions_ProductInventoryId",
                table: "EntityVersions",
                column: "ProductInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityVersions_ProductInventoryReservationId",
                table: "EntityVersions",
                column: "ProductInventoryReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityVersions_ProductInventoryReservationItemId",
                table: "EntityVersions",
                column: "ProductInventoryReservationItemId");

            migrationBuilder.CreateIndex(
                name: "UX_ProductInventories_ProductId",
                table: "ProductInventories",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventoryReservationItems_ProductId",
                table: "ProductInventoryReservationItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "UX_ProductInventoryReservationItems_ReservationId_ProductId",
                table: "ProductInventoryReservationItems",
                columns: new[] { "ProductInventoryReservationId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ProductInventoryReservations_OrderId",
                table: "ProductInventoryReservations",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityVersions_ProductInventories_ProductInventoryId",
                table: "EntityVersions",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityVersions_ProductInventoryReservationItems_ProductInventoryReservationItemId",
                table: "EntityVersions",
                column: "ProductInventoryReservationItemId",
                principalTable: "ProductInventoryReservationItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityVersions_ProductInventoryReservations_ProductInventoryReservationId",
                table: "EntityVersions",
                column: "ProductInventoryReservationId",
                principalTable: "ProductInventoryReservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityVersions_ProductInventories_ProductInventoryId",
                table: "EntityVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityVersions_ProductInventoryReservationItems_ProductInventoryReservationItemId",
                table: "EntityVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityVersions_ProductInventoryReservations_ProductInventoryReservationId",
                table: "EntityVersions");

            migrationBuilder.DropTable(
                name: "ProductInventories");

            migrationBuilder.DropTable(
                name: "ProductInventoryReservationItems");

            migrationBuilder.DropTable(
                name: "ProductInventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_EntityVersions_ProductInventoryId",
                table: "EntityVersions");

            migrationBuilder.DropIndex(
                name: "IX_EntityVersions_ProductInventoryReservationId",
                table: "EntityVersions");

            migrationBuilder.DropIndex(
                name: "IX_EntityVersions_ProductInventoryReservationItemId",
                table: "EntityVersions");

            migrationBuilder.DropColumn(
                name: "ProductInventoryId",
                table: "EntityVersions");

            migrationBuilder.DropColumn(
                name: "ProductInventoryReservationId",
                table: "EntityVersions");

            migrationBuilder.DropColumn(
                name: "ProductInventoryReservationItemId",
                table: "EntityVersions");
        }
    }
}
