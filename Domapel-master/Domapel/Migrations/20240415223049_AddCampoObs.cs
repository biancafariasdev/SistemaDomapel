using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domapel.Migrations
{
    /// <inheritdoc />
    public partial class AddCampoObs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
            name: "Observations",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

            migrationBuilder.AddColumn<int>(
            name: "Payment",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
