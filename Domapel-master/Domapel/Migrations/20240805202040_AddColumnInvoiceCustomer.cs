using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domapel.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnInvoiceCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "HasInvoice",
               table: "Customers",
               type: "int",
               nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
