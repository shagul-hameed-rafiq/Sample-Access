using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLabAInsights.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitFlowTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visit",
                columns: table => new
                {
                    VisitId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    VisitDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PanelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<int>(type: "INTEGER", nullable: true),
                    Systolic = table.Column<int>(type: "INTEGER", nullable: true),
                    Diastolic = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visit", x => x.VisitId);
                    table.ForeignKey(
                        name: "FK_Visit_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visit_Panel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panel",
                        principalColumn: "PanelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitPanelSummary",
                columns: table => new
                {
                    VisitPanelSummaryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitId = table.Column<int>(type: "INTEGER", nullable: false),
                    PanelRuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    StandardSummarySnapshot = table.Column<string>(type: "TEXT", nullable: false),
                    RevisedSummary = table.Column<string>(type: "TEXT", nullable: true),
                    IsRevised = table.Column<bool>(type: "INTEGER", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitPanelSummary", x => x.VisitPanelSummaryId);
                    table.ForeignKey(
                        name: "FK_VisitPanelSummary_PanelRuleSummary_PanelRuleId",
                        column: x => x.PanelRuleId,
                        principalTable: "PanelRuleSummary",
                        principalColumn: "PanelRuleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitPanelSummary_Visit_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visit",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitTestResult",
                columns: table => new
                {
                    VisitTestResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestId = table.Column<int>(type: "INTEGER", nullable: false),
                    RawValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EnteredAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitTestResult", x => x.VisitTestResultId);
                    table.ForeignKey(
                        name: "FK_VisitTestResult_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitTestResult_Visit_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visit",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitTestInterpretation",
                columns: table => new
                {
                    VisitTestInterpretationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitTestResultId = table.Column<int>(type: "INTEGER", nullable: false),
                    BandId = table.Column<int>(type: "INTEGER", nullable: false),
                    StandardReportSnapshot = table.Column<string>(type: "TEXT", nullable: false),
                    RevisedReport = table.Column<string>(type: "TEXT", nullable: true),
                    IsRevised = table.Column<bool>(type: "INTEGER", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitTestInterpretation", x => x.VisitTestInterpretationId);
                    table.ForeignKey(
                        name: "FK_VisitTestInterpretation_BandRuleReport_BandId",
                        column: x => x.BandId,
                        principalTable: "BandRuleReport",
                        principalColumn: "BandId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitTestInterpretation_VisitTestResult_VisitTestResultId",
                        column: x => x.VisitTestResultId,
                        principalTable: "VisitTestResult",
                        principalColumn: "VisitTestResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visit_MemberId_VisitDateTime",
                table: "Visit",
                columns: new[] { "MemberId", "VisitDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Visit_PanelId",
                table: "Visit",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPanelSummary_EvaluatedAt",
                table: "VisitPanelSummary",
                column: "EvaluatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPanelSummary_PanelRuleId",
                table: "VisitPanelSummary",
                column: "PanelRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPanelSummary_VisitId",
                table: "VisitPanelSummary",
                column: "VisitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestInterpretation_BandId",
                table: "VisitTestInterpretation",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestInterpretation_EvaluatedAt",
                table: "VisitTestInterpretation",
                column: "EvaluatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestInterpretation_VisitTestResultId",
                table: "VisitTestInterpretation",
                column: "VisitTestResultId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestResult_TestId",
                table: "VisitTestResult",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestResult_VisitId_TestId",
                table: "VisitTestResult",
                columns: new[] { "VisitId", "TestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitPanelSummary");

            migrationBuilder.DropTable(
                name: "VisitTestInterpretation");

            migrationBuilder.DropTable(
                name: "VisitTestResult");

            migrationBuilder.DropTable(
                name: "Visit");
        }
    }
}
