using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_API.Migrations
{
    public partial class RestrictionsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestrictionsByDomain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictionsByDomain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestrictionsByDomain_APIKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "APIKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestrictionsByIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictionsByIP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestrictionsByIP_APIKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "APIKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionsByDomain_KeyId",
                table: "RestrictionsByDomain",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionsByIP_KeyId",
                table: "RestrictionsByIP",
                column: "KeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestrictionsByDomain");

            migrationBuilder.DropTable(
                name: "RestrictionsByIP");
        }
    }
}
