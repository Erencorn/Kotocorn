﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Kotocorn.Migrations
{
    public partial class verboseerrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VerboseErrors",
                table: "GuildConfigs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerboseErrors",
                table: "GuildConfigs");
        }
    }
}