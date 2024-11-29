using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domapel.Migrations
{
    /// <inheritdoc />
    public partial class AlterarTipoDeColunaCommissionValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AlterColumn<string>(
                name: "CommissionValue",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Observations",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ValueItem",
                table: "OrderItems");
        }
    }
}
