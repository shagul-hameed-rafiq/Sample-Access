using Microsoft.ML.Data;

namespace MedLabAInsights.ML.Models
{
    public class DiabetesPanelSummaryTrainingData
    {
        [LoadColumn(0)]
        public string Record { get; set; }

        [LoadColumn(1)]
        public float PanelId { get; set; }

        [LoadColumn(2)]
        public string PanelCode { get; set; }

        [LoadColumn(3)]
        public string PanelRuleCode { get; set; }

        [LoadColumn(4)]
        public string PanelRuleName { get; set; }

        [LoadColumn(5)]
        public float MinSeverity { get; set; }

        [LoadColumn(6)]
        public float MaxSeverity { get; set; }

        [LoadColumn(7)]
        public string RevisedSummary { get; set; }

        [LoadColumn(8)]
        public string SummaryVariantCode { get; set; }
    }
}