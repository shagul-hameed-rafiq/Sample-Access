using MedLabAInsights.DataGenerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MedLabAInsights.DataGenerator.Services
{
    public class DiabetesSyntheticDataGenerator
    {
        private readonly Random _random = new Random();

        public List<DiabetesTrainingRow> Generate(int normalCount, int prediabetesCount, int diabetesCount)
        {
            var rows = new List<DiabetesTrainingRow>();

            rows.AddRange(GenerateNormalRows(normalCount));
            rows.AddRange(GeneratePrediabetesRows(prediabetesCount));
            rows.AddRange(GenerateDiabetesRows(diabetesCount));

            // Shuffle rows
            return rows.OrderBy(x => _random.Next()).ToList();
        }

        public void WriteToCsv(string filePath, List<DiabetesTrainingRow> rows)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Age,Gender,BMI,HbA1c,FastingGlucose,PostPrandialGlucose,MeanBloodGlucose,RiskLabel");

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",",
                    row.Age,
                    row.Gender,
                    row.BMI.ToString("0.0", CultureInfo.InvariantCulture),
                    row.HbA1c.ToString("0.0", CultureInfo.InvariantCulture),
                    row.FastingGlucose.ToString("0.0", CultureInfo.InvariantCulture),
                    row.PostPrandialGlucose.ToString("0.0", CultureInfo.InvariantCulture),
                    row.MeanBloodGlucose.ToString("0.0", CultureInfo.InvariantCulture),
                    row.RiskLabel));
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        private IEnumerable<DiabetesTrainingRow> GenerateNormalRows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                float hbA1c = NextFloat(4.0f, 5.6f);

                yield return new DiabetesTrainingRow
                {
                    Age = _random.Next(18, 41),
                    Gender = _random.Next(0, 2),
                    BMI = NextFloat(18.5f, 24.9f),
                    HbA1c = hbA1c,
                    FastingGlucose = NextFloat(70f, 99f),
                    PostPrandialGlucose = NextFloat(80f, 139f),
                    MeanBloodGlucose = CalculateMeanBloodGlucose(hbA1c),
                    RiskLabel = 0
                };
            }
        }

        private IEnumerable<DiabetesTrainingRow> GeneratePrediabetesRows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                float hbA1c = NextFloat(5.7f, 6.4f);

                yield return new DiabetesTrainingRow
                {
                    Age = _random.Next(25, 56),
                    Gender = _random.Next(0, 2),
                    BMI = NextFloat(23f, 30f),
                    HbA1c = hbA1c,
                    FastingGlucose = NextFloat(100f, 125f),
                    PostPrandialGlucose = NextFloat(140f, 199f),
                    MeanBloodGlucose = CalculateMeanBloodGlucose(hbA1c),
                    RiskLabel = 1
                };
            }
        }

        private IEnumerable<DiabetesTrainingRow> GenerateDiabetesRows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                float hbA1c = NextFloat(6.5f, 11.0f);

                yield return new DiabetesTrainingRow
                {
                    Age = _random.Next(30, 71),
                    Gender = _random.Next(0, 2),
                    BMI = NextFloat(25f, 38f),
                    HbA1c = hbA1c,
                    FastingGlucose = NextFloat(126f, 220f),
                    PostPrandialGlucose = NextFloat(200f, 350f),
                    MeanBloodGlucose = CalculateMeanBloodGlucose(hbA1c),
                    RiskLabel = 2
                };
            }
        }

        private float CalculateMeanBloodGlucose(float hbA1c)
        {
            return (float)Math.Round((28.7f * hbA1c) - 46.7f, 1);
        }

        private float NextFloat(float min, float max)
        {
            return (float)(_random.NextDouble() * (max - min) + min);
        }
    }
}