using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLabAInsights.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Panel_Test_MasterAndMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Panel",
                columns: table => new
                {
                    PanelId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PanelName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PanelCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel", x => x.PanelId);
                });

            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TestCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MinValue = table.Column<double>(type: "REAL", nullable: true),
                    MaxValue = table.Column<double>(type: "REAL", nullable: true),
                    CustomValues = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.TestId);
                });

            migrationBuilder.CreateTable(
                name: "PanelTestMapping",
                columns: table => new
                {
                    PanelTestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PanelId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImportanceLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelTestMapping", x => x.PanelTestId);
                    table.ForeignKey(
                        name: "FK_PanelTestMapping_Panel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panel",
                        principalColumn: "PanelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PanelTestMapping_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PanelTestMapping_PanelId_TestId",
                table: "PanelTestMapping",
                columns: new[] { "PanelId", "TestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PanelTestMapping_TestId",
                table: "PanelTestMapping",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Test_TestName",
                table: "Test",
                column: "TestName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PanelTestMapping");

            migrationBuilder.DropTable(
                name: "Panel");

            migrationBuilder.DropTable(
                name: "Test");
        }
    }
}
