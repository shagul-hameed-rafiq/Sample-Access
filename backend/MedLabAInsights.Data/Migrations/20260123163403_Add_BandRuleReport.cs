using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLabAInsights.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_BandRuleReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BandRuleReport",
                columns: table => new
                {
                    BandId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestId = table.Column<int>(type: "INTEGER", nullable: false),
                    BandName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BandCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RangeMin = table.Column<double>(type: "REAL", nullable: false),
                    RangeMax = table.Column<double>(type: "REAL", nullable: false),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    StandardReport = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandRuleReport", x => x.BandId);
                    table.ForeignKey(
                        name: "FK_BandRuleReport_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BandRuleReport_TestId",
                table: "BandRuleReport",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_BandRuleReport_TestId_BandCode",
                table: "BandRuleReport",
                columns: new[] { "TestId", "BandCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BandRuleReport");
        }
    }
}
