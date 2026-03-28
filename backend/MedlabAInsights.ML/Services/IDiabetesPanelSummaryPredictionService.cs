namespace MedLabAInsights.ML.Services
{
    public interface IDiabetesPanelSummaryPredictionService
    {
        string PredictSummary(string panelRuleCode, float minSeverity, float maxSeverity);
    }
}