using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domapel.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposCEP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Complement",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Vendors_VendorId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_VendorId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Complement",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Customers");
        }
    }
}
