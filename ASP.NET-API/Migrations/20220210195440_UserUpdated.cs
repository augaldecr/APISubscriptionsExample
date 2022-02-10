using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_API.Migrations
{
    public partial class UserUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "debtor",
                table: "AspNetUsers",
                newName: "Debtor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Debtor",
                table: "AspNetUsers",
                newName: "debtor");
        }
    }
}
