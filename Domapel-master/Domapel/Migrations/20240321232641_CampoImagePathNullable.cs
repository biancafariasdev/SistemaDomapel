using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domapel.Migrations
{
    /// <inheritdoc />
    public partial class CampoImagePathNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
            name: "ImagePath",
            table: "Customers",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
