using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLabAInsights.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_PanelRuleSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PanelRuleSummary",
                columns: table => new
                {
                    PanelRuleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PanelId = table.Column<int>(type: "INTEGER", nullable: false),
                    PanelRuleName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PanelRuleCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MinSeverity = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxSeverity = table.Column<int>(type: "INTEGER", nullable: false),
                    StandardSummary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelRuleSummary", x => x.PanelRuleId);
                    table.ForeignKey(
                        name: "FK_PanelRuleSummary_Panel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panel",
                        principalColumn: "PanelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PanelRuleSummary_PanelId",
                table: "PanelRuleSummary",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_PanelRuleSummary_PanelId_PanelRuleCode",
                table: "PanelRuleSummary",
                columns: new[] { "PanelId", "PanelRuleCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PanelRuleSummary");
        }
    }
}
