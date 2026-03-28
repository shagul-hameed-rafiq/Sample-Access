namespace MedLabAInsights.ML.Helpers
{
    public static class DiabetesSummaryTextBuilder
    {
        public static string Build(string panelRuleCode, float minSeverity, float maxSeverity)
        {
            return $"RuleCode:{panelRuleCode} MinSeverity:{minSeverity} MaxSeverity:{maxSeverity}";
        }
    }
}