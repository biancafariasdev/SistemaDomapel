using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domapel.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceAndCommissionValueColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Vendors_VendorId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_VendorId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CommissionValue",
                table: "OrderItems");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Invoice",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionValue",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Invoice",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_VendorId",
                table: "Customers",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Vendors_VendorId",
                table: "Customers",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
