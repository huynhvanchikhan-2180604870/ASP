using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSuppliesStore.Migrations
{
    /// <inheritdoc />
    public partial class updateOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Orders",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");
        }
    }
}
