using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLabAInsights.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Member : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BloodGroup = table.Column<int>(type: "INTEGER", nullable: false),
                    Contact = table.Column<long>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Member_Contact",
                table: "Member",
                column: "Contact",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Member");
        }
    }
}
