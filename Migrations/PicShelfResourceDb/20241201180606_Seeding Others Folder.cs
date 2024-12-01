using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicShelfServer.Migrations.PicShelfResourceDb
{
    /// <inheritdoc />
    public partial class SeedingOthersFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Folders",
                columns: new[] { "Id", "FolderName" },
                values: new object[] { new Guid("e45fd846-f89b-44f9-9372-fc24c3395cea"), "Others" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: new Guid("e45fd846-f89b-44f9-9372-fc24c3395cea"));
        }
    }
}
