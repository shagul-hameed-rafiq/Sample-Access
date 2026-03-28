using Microsoft.ML.Data;

namespace MedLabAInsights.ML.Models
{
    public class DiabetesPanelSummaryPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedSummary { get; set; }

        public float[] Score { get; set; }
    }
}