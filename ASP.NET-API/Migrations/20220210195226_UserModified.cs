using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_API.Migrations
{
    public partial class UserModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "debtor",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "debtor",
                table: "AspNetUsers");
        }
    }
}
