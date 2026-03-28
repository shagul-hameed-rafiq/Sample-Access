namespace MedLabAInsights.ML.Models
{
    public class DiabetesPanelSummaryModelInput
    {
        public string InputText { get; set; }

        // Dummy value needed because saved pipeline expects this column
        public string SummaryLabel { get; set; }
    }
}