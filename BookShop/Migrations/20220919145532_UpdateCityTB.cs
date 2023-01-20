using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShop.Migrations
{
    public partial class UpdateCityTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProvinceID",
                table: "Cities",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Delete",
                table: "BookInfo",
                nullable: true,
                defaultValueSql: "0",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvinceID",
                table: "Cities");

            migrationBuilder.AlterColumn<bool>(
                name: "Delete",
                table: "BookInfo",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValueSql: "0");
        }
    }
}
