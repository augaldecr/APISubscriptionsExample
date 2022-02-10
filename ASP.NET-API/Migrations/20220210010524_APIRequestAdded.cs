﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_API.Migrations
{
    public partial class APIRequestAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    APIKeyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APIRequests_APIKeys_APIKeyId",
                        column: x => x.APIKeyId,
                        principalTable: "APIKeys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_APIRequests_APIKeyId",
                table: "APIRequests",
                column: "APIKeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIRequests");
        }
    }
}
