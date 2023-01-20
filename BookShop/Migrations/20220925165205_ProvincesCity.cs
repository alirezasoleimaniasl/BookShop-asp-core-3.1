using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShop.Migrations
{
    public partial class ProvincesCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provices_ProviceProvinceID",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "Provices");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ProviceProvinceID",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ProviceProvinceID",
                table: "Cities");

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvinceID = table.Column<int>(nullable: false),
                    ProvinceName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceID",
                table: "Cities",
                column: "ProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provinces_ProvinceID",
                table: "Cities",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provinces_ProvinceID",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ProvinceID",
                table: "Cities");

            migrationBuilder.AddColumn<int>(
                name: "ProviceProvinceID",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provices",
                columns: table => new
                {
                    ProvinceID = table.Column<int>(type: "int", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provices", x => x.ProvinceID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProviceProvinceID",
                table: "Cities",
                column: "ProviceProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provices_ProviceProvinceID",
                table: "Cities",
                column: "ProviceProvinceID",
                principalTable: "Provices",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
