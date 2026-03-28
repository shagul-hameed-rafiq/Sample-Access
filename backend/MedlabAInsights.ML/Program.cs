using MedLabAInsights.ML.Helpers;
using MedLabAInsights.ML.Services;
using MedLabAInsights.ML.Training;

string dataPath = PathHelper.GetDataFilePath("diabetes_panel_summary_training.csv");
string modelPath = PathHelper.GetModelFilePath("diabetes_panel_summary_model.zip");

Console.WriteLine($"CSV Path   : {dataPath}");
Console.WriteLine($"Model Path : {modelPath}");

// Train
var trainer = new DiabetesPanelSummaryTrainer();
trainer.TrainAndSaveModel(dataPath, modelPath);

// Predict
var predictionService = new DiabetesPanelSummaryPredictionService(modelPath);

string predictedSummary = predictionService.PredictSummary(
    panelRuleCode: "DIA_DM_MODERATE",
    minSeverity: 6,
    maxSeverity: 6);

Console.WriteLine();
Console.WriteLine("Predicted Summary:");
Console.WriteLine(predictedSummary);