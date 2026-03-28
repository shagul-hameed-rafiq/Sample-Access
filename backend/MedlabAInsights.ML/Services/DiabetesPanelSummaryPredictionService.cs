using Microsoft.ML;
using MedLabAInsights.ML.Helpers;
using MedLabAInsights.ML.Models;

namespace MedLabAInsights.ML.Services
{
    public class DiabetesPanelSummaryPredictionService : IDiabetesPanelSummaryPredictionService
    {
        private readonly PredictionEngine<DiabetesPanelSummaryModelInput, DiabetesPanelSummaryPrediction> _predictionEngine;

        public DiabetesPanelSummaryPredictionService(string modelPath)
        {
            var mlContext = new MLContext();

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException($"Model file not found: {modelPath}");
            }

            var model = mlContext.Model.Load(modelPath, out _);

            _predictionEngine =
                mlContext.Model.CreatePredictionEngine<DiabetesPanelSummaryModelInput, DiabetesPanelSummaryPrediction>(model);
        }

        public string PredictSummary(string panelRuleCode, float minSeverity, float maxSeverity)
        {
            var input = new DiabetesPanelSummaryModelInput
            {
                InputText = DiabetesSummaryTextBuilder.Build(panelRuleCode, minSeverity, maxSeverity),
                SummaryLabel = string.Empty
            };

            var prediction = _predictionEngine.Predict(input);

            return prediction.PredictedSummary;
        }
    }
}