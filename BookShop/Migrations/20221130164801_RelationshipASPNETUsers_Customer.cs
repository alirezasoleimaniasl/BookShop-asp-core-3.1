using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShop.Migrations
{
    public partial class RelationshipASPNETUsers_Customer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(

                name: "FK_Customers_AspNetUsers_CustomerID",
                table:"Customers",
                column:"CustomerID",
                principalTable:"AspNetUsers",
                principalColumn:"Id",
                onDelete:ReferentialAction.NoAction
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_CustomerID",
                table: "Customers"
                );
        }
    }
}
