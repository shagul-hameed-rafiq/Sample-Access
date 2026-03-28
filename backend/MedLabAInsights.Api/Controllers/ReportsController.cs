using MedLabAInsights.Api.Contracts.Reports;
using MedLabAInsights.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/visits")]
public sealed class ReportsController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public ReportsController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // GET: api/visits/{visitId}/report
    [HttpGet("{visitId:int}/report")]
    public async Task<ActionResult<VisitReportDto>> GetVisitReport(int visitId, CancellationToken ct)
    {
        // 1) Header: Visit + Member + Panel + vitals
        var header = await (
            from v in _db.Visits.AsNoTracking()
            join m in _db.Members.AsNoTracking() on v.MemberId equals m.MemberId
            join p in _db.Panels.AsNoTracking() on v.PanelId equals p.PanelId
            where v.VisitId == visitId
            select new
            {
                v.VisitId,
                v.VisitDateTime,

                Member = new
                {
                    m.MemberId,
                    m.Name,
                    Gender = m.Gender.ToString(),
                    m.DateOfBirth,
                    BloodGroup = m.BloodGroup.ToString(),
                    m.Contact,
                    m.Address
                },

                Panel = new
                {
                    p.PanelId,
                    p.PanelName,
                    p.PanelCode
                },

                Vitals = new
                {
                    v.Height,
                    v.Weight,
                    v.Systolic,
                    v.Diastolic
                }
            }
        ).FirstOrDefaultAsync(ct);

        if (header is null)
            return NotFound(new { message = $"Visit {visitId} not found" });

        // 2) Panel summary (must exist for "Submitted")
        var panelSummary = await (
            from vps in _db.VisitPanelSummaries.AsNoTracking()
            join prs in _db.PanelRuleSummaries.AsNoTracking() on vps.PanelRuleId equals prs.PanelRuleId
            where vps.VisitId == visitId
            select new
            {
                vps.VisitPanelSummaryId,
                prs.PanelRuleId,
                prs.PanelRuleName,
                prs.MinSeverity,
                prs.MaxSeverity,
                StandardSummary = vps.StandardSummarySnapshot,
                vps.RevisedSummary,
                vps.IsRevised
            }
        ).FirstOrDefaultAsync(ct);

        var status = panelSummary is null ? "Draft" : "Submitted";

        // If Draft, you can still return partial response (no panel summary/tests interpretations)
        if (panelSummary is null)
        {
            return Ok(new VisitReportDto
            {
                VisitId = header.VisitId,
                VisitDateTime = header.VisitDateTime,
                Status = status,
                Member = new MemberHeaderDto
                {
                    MemberId = header.Member.MemberId,
                    Name = header.Member.Name,
                    Gender = header.Member.Gender,
                    Age = CalculateAge(header.Member.DateOfBirth, header.VisitDateTime),
                    BloodGroup = header.Member.BloodGroup,
                    Contact = header.Member.Contact,
                    Address = header.Member.Address
                },
                Panel = new PanelHeaderDto
                {
                    PanelId = header.Panel.PanelId,
                    PanelName = header.Panel.PanelName,
                    PanelCode = header.Panel.PanelCode
                },
                Vitals = new VitalsDto
                {
                    Height = header.Vitals.Height,
                    Weight = header.Vitals.Weight,
                    Systolic = header.Vitals.Systolic,
                    Diastolic = header.Vitals.Diastolic
                },
                PanelSummary = new PanelSummaryReportDto
                {
                    PanelRuleId = 0,
                    PanelRuleName = "",
                    MinSeverity = 0,
                    MaxSeverity = 0,
                    StandardSummary = "",
                    RevisedSummary = null,
                    IsRevised = false,
                    EffectiveSummary = ""
                },
                Tests = new List<TestReportDto>()
            });
        }

        var testRows = await (
            from r in _db.VisitTestResults.AsNoTracking()
            join t in _db.Tests.AsNoTracking() on r.TestId equals t.TestId
            join i in _db.VisitTestInterpretations.AsNoTracking() on r.VisitTestResultId equals i.VisitTestResultId
            join b in _db.BandRuleReports.AsNoTracking() on i.BandId equals b.BandId
            where r.VisitId == visitId
            select new
            {
                TestId       = t.TestId,
                TestName     = t.TestName,
                TestCode     = t.TestCode!,
                Unit         = t.Unit!,
                MinValue     = t.MinValue,
                MaxValue     = t.MaxValue,
                RawValue     = r.RawValue,
                BandId       = b.BandId,
                BandName     = b.BandName,
                BandCode     = b.BandCode,
                Severity     = b.Severity,
                StandardReport   = i.StandardReportSnapshot,
                RevisedReport    = i.RevisedReport,
                IsRevised        = i.IsRevised,
                EffectiveReport  = i.IsRevised && i.RevisedReport != null && i.RevisedReport != ""
                    ? i.RevisedReport
                    : i.StandardReportSnapshot
            }
        ).ToListAsync(ct);

        // 1) Find HbA1c value for context to evaluate Mean BGL
        double? hba1cValue = null;
        var hba1cRow = testRows.FirstOrDefault(r => r.TestCode == "DIA_HBA1C");
        if (hba1cRow != null && double.TryParse(hba1cRow.RawValue, out var hba1c))
        {
            hba1cValue = hba1c;
        }

        // Derive Status from BandCode (explicit per-test medical classification)
        var tests = testRows.Select(row => 
        {
            var rawValueStr = row.RawValue ?? "";
            var status = DeriveStatus(row.TestCode, row.BandCode, row.Severity, rawValueStr, hba1cValue);
            var standardReport = row.StandardReport;
            var effectiveReport = row.EffectiveReport;

            // Requirement: Override interpretation text for Mean Blood Glucose if it shows 140-199 range
            if (row.TestCode == "DIA_MBGL" && double.TryParse(rawValueStr, out var mbgl))
            {
                if (mbgl >= 140 && mbgl <= 199)
                {
                    standardReport = "Elevated average glucose indicating diabetes; control required";
                    if (!row.IsRevised) 
                    {
                        effectiveReport = standardReport;
                    }
                }
            }

            // CBC Interpretation enhancements (appended dynamically)
            if (row.TestCode != null && row.TestCode.StartsWith("CBC_"))
            {
                var suffix = "";
                if (status.Contains("CRITICAL")) suffix = " - urgent attention.";
                else if (status.Contains("HIGH")) suffix = " - possible condition.";
                else if (status.Contains("LOW")) suffix = " - deficiency indication.";

                if (!string.IsNullOrEmpty(suffix) && !standardReport.Contains(suffix.Trim()))
                {
                    standardReport = standardReport.TrimEnd('.') + suffix;
                    if (!row.IsRevised)
                    {
                        effectiveReport = standardReport;
                    }
                }
            }

            return new TestReportDto
            {
                TestId       = row.TestId,
                TestName     = row.TestName,
                TestCode     = row.TestCode,
                Unit         = row.Unit,
                MinValue     = row.MinValue,
                MaxValue     = row.MaxValue,
                RawValue     = row.RawValue,
                BandId       = row.BandId,
                BandName     = row.BandName,
                BandCode     = row.BandCode,
                Severity     = row.Severity,
                Status       = status,
                StandardReport  = standardReport,
                RevisedReport   = row.RevisedReport,
                IsRevised       = row.IsRevised,
                EffectiveReport = effectiveReport
            };
        }).ToList();

        // Order: worst first, then name
        tests = tests
            .OrderByDescending(x => x.Severity)
            .ThenBy(x => x.TestName)
            .ToList();

        // 4) Compose response
        var response = new VisitReportDto
        {
            VisitId = header.VisitId,
            VisitDateTime = header.VisitDateTime,
            Status = status,

            Member = new MemberHeaderDto
            {
                MemberId = header.Member.MemberId,
                Name = header.Member.Name,
                Gender = header.Member.Gender,
                Age = CalculateAge(header.Member.DateOfBirth, header.VisitDateTime),
                BloodGroup = header.Member.BloodGroup,
                Contact = header.Member.Contact,
                Address = header.Member.Address
            },

            Panel = new PanelHeaderDto
            {
                PanelId = header.Panel.PanelId,
                PanelName = header.Panel.PanelName,
                PanelCode = header.Panel.PanelCode
            },

            Vitals = new VitalsDto
            {
                Height = header.Vitals.Height,
                Weight = header.Vitals.Weight,
                Systolic = header.Vitals.Systolic,
                Diastolic = header.Vitals.Diastolic
            },

            PanelSummary = new PanelSummaryReportDto
            {
                PanelRuleId = panelSummary.PanelRuleId,
                PanelRuleName = panelSummary.PanelRuleName,
                MinSeverity = panelSummary.MinSeverity,
                MaxSeverity = panelSummary.MaxSeverity,
                StandardSummary = panelSummary.StandardSummary,
                RevisedSummary = panelSummary.RevisedSummary,
                IsRevised = panelSummary.IsRevised,
                EffectiveSummary = panelSummary.IsRevised && !string.IsNullOrWhiteSpace(panelSummary.RevisedSummary)
                    ? panelSummary.RevisedSummary!
                    : panelSummary.StandardSummary
            },

            Tests = tests
        };

        return Ok(response);
    }

    private static int CalculateAge(DateTime dob, DateTime onDate)
    {
        var age = onDate.Year - dob.Year;
        if (dob.Date > onDate.Date.AddYears(-age)) age--;
        return age < 0 ? 0 : age;
    }

    /// <summary>
    /// Derives a human-readable Status label from TestCode + BandCode + Severity + RawValue + context.
    /// Diabetes tests use explicit clinical classification (ADA criteria).
    /// All other tests use severity tiers: LOW / NORMAL / HIGH / CRITICAL.
    /// </summary>
    private static string DeriveStatus(string testCode, string bandCode, int severity, string rawValue, double? hba1cValue = null)
    {
        var tc = (testCode ?? "").ToUpperInvariant();
        var bc = (bandCode ?? "").ToUpperInvariant();

        // ── Diabetes panel: FBS, PPBS, HbA1c, Mean BGL ──────────────────
        if (tc.StartsWith("DIA_"))
        {
            // Hypoglycemia bands (Keep safety critical alarms)
            if (bc.Contains("HYPOGLYCEMIA_SEVERE")) return "CRITICAL-LOW";
            if (bc.Contains("HYPOGLYCEMIA_MILD"))   return "LOW";

            // Mean Blood Glucose Specific Clinical Classification Override
            if (tc == "DIA_MBGL" && double.TryParse(rawValue, out var mbgl))
            {
                if (mbgl >= 140) return "DIABETIC"; // 140-199 Moderate, >=200 Poor Control
                
                // If it's < 140, ensure we do NOT classify as Pre-Diabetic if HbA1c >= 6.5
                if (hba1cValue.HasValue && hba1cValue.Value >= 6.5)
                {
                    return "NORMAL"; // Contradicts HbA1c; we remove Pre-diabetic classification
                }
                return "NORMAL";
            }

            // Normal / optimal
            if (bc.Contains("_OPTIMAL") || bc.Contains("_NORMAL")) return "NORMAL";

            // Pre-diabetes
            bool isPreDiabetic = bc.Contains("PREDIABETES") || bc.Contains("HIGH_NORMAL");
            if (isPreDiabetic)
            {
                if (tc == "DIA_MBGL" && hba1cValue.HasValue && hba1cValue.Value >= 6.5)
                {
                    return "DIABETIC"; // Redundancy catch if TryParse somehow failed
                }
                return "PRE-DIABETIC";
            }

            // Diabetes
            if (bc.Contains("DIABETES_THRESHOLD") ||
                bc.Contains("DIABETES_MODERATE")  ||
                bc.Contains("DIABETES_SEVERE")    ||
                bc.Contains("DIABETES_POOR")      ||
                bc.Contains("EXTREME"))               return "DIABETIC";

            return "NORMAL"; // safe fallback for diabetes
        }

        // ── Thyroid panel: TSH, T3, T4 ────────────────────────────────
        if (tc.StartsWith("THY_"))
        {
            if (bc.Contains("_NORMAL"))      return "NORMAL";
            if (bc.Contains("_SEVERE_LOW") ||
                bc.Contains("_CRITICAL_LOW")) return "CRITICAL-LOW";
            if (bc.Contains("_LOW"))         return "LOW";
            if (bc.Contains("_SEVERE_HIGH") ||
                bc.Contains("_VERY_HIGH"))   return "CRITICAL-HIGH";
            if (bc.Contains("_MILD_HIGH") ||
                bc.Contains("_HIGH"))        return "HIGH";
            return "NORMAL";
        }

        // ── CBC panel: all CBC_ tests ────────────────────────────────────
        if (tc.StartsWith("CBC_"))
        {
            // Explicit example thresholds for Hemoglobin
            if (tc == "CBC_HB" && double.TryParse(rawValue, out var hb))
            {
                if (hb < 8) return "CRITICAL-LOW";
                if (hb >= 8 && hb < 12) return "LOW";
                if (hb > 17) return "HIGH";
                return "NORMAL"; // 12-17
            }

            if (bc.Contains("_NORMAL"))      return "NORMAL";
            if (bc.Contains("_SEVERE_LOW"))  return "CRITICAL-LOW";
            if (bc.Contains("_LOW"))         return "LOW";
            if (bc.Contains("_VERY_HIGH"))   return "CRITICAL-HIGH";
            if (bc.Contains("_HIGH"))        return "HIGH";
            return "NORMAL";
        }

        // ── Generic fallback using severity tier ─────────────────────────
        return severity switch
        {
            0 or 1 or 2 => "NORMAL",
            3 or 4       => "HIGH",
            5 or 6       => "CRITICAL-HIGH",
            _            => "CRITICAL"
        };
    }
}