using Microsoft.ML;
using MedLabAInsights.ML.Helpers;
using MedLabAInsights.ML.Models;

namespace MedLabAInsights.ML.Training
{
    public class DiabetesPanelSummaryTrainer
    {
        private readonly MLContext _mlContext;

        public DiabetesPanelSummaryTrainer()
        {
            _mlContext = new MLContext(seed: 1);
        }

        public void TrainAndSaveModel(string dataPath, string modelPath)
        {
            IDataView rawData = _mlContext.Data.LoadFromTextFile<DiabetesPanelSummaryTrainingData>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true);

            var rawRows = _mlContext.Data
                .CreateEnumerable<DiabetesPanelSummaryTrainingData>(rawData, reuseRowObject: false)
                .ToList();

            var trainingRows = rawRows.Select(r => new DiabetesSummaryModelRow
            {
                InputText = DiabetesSummaryTextBuilder.Build(r.PanelRuleCode, r.MinSeverity, r.MaxSeverity),
                SummaryLabel = r.RevisedSummary
            });

            IDataView trainingData = _mlContext.Data.LoadFromEnumerable(trainingRows);

            var pipeline =
                _mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "Label",
                    inputColumnName: nameof(DiabetesSummaryModelRow.SummaryLabel))
                .Append(_mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(DiabetesSummaryModelRow.InputText)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: "Label",
                    featureColumnName: "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(trainingData);

            var predictions = model.Transform(trainingData);

            var metrics = _mlContext.MulticlassClassification.Evaluate(
                predictions,
                labelColumnName: "Label",
                predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy:P2}");
            Console.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy:P2}");

            _mlContext.Model.Save(model, trainingData.Schema, modelPath);

            Console.WriteLine($"Model saved at: {modelPath}");
        }
    }
}