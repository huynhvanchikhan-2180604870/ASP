using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSuppliesStore.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabaseUpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSaleOff",
                table: "Products",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSaleOff",
                table: "Products");
        }
    }
}
