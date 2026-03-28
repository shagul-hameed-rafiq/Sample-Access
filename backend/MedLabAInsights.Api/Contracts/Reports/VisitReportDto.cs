namespace MedLabAInsights.Api.Contracts.Reports;

public sealed class VisitReportDto
{
    public int VisitId { get; init; }
    public DateTime VisitDateTime { get; init; }

    public MemberHeaderDto Member { get; init; } = null!;
    public PanelHeaderDto Panel { get; init; } = null!;
    public VitalsDto Vitals { get; init; } = null!;

    public string Status { get; init; } = null!; // Draft / Submitted

    public PanelSummaryReportDto PanelSummary { get; init; } = null!;
    public List<TestReportDto> Tests { get; init; } = new();
}

public sealed class MemberHeaderDto
{
    public int MemberId { get; init; }
    public string Name { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public int Age { get; init; }
    public string BloodGroup { get; init; } = null!;
    public long Contact { get; init; }
    public string? Address { get; init; }
}

public sealed class PanelHeaderDto
{
    public int PanelId { get; init; }
    public string PanelName { get; init; } = null!;
    public string PanelCode { get; init; } = null!;
}

public sealed class VitalsDto
{
    public int? Height { get; init; }
    public int? Weight { get; init; }
    public int? Systolic { get; init; }
    public int? Diastolic { get; init; }
}

public sealed class PanelSummaryReportDto
{
    public int PanelRuleId { get; init; }
    public string PanelRuleName { get; init; } = null!;
    public int MinSeverity { get; init; }
    public int MaxSeverity { get; init; }

    public string StandardSummary { get; init; } = null!;
    public string? RevisedSummary { get; init; }
    public bool IsRevised { get; init; }

    public string EffectiveSummary { get; init; } = null!;
}

public sealed class TestReportDto
{
    public int TestId { get; init; }
    public string TestName { get; init; } = null!;
    public string TestCode { get; init; } = null!;
    public string Unit { get; init; } = null!;
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }

    public string RawValue { get; init; } = null!;

    public int BandId { get; init; }
    public string BandName { get; init; } = null!;
    public string BandCode { get; init; } = null!;
    public int Severity { get; init; }

    /// <summary>
    /// Human-readable classification derived from Severity:
    /// NORMAL | PRE-DIABETIC | DIABETIC | LOW | HIGH | CRITICAL
    /// </summary>
    public string Status { get; init; } = null!;

    public string StandardReport { get; init; } = null!;
    public string? RevisedReport { get; init; }
    public bool IsRevised { get; init; }

    public string EffectiveReport { get; init; } = null!;
}