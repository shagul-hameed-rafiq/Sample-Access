namespace MedLabAInsights.DataGenerator.Models
{
    public class DiabetesTrainingRow
    {
        public int Age { get; set; }

        public int Gender { get; set; }

        public float BMI { get; set; }

        public float HbA1c { get; set; }

        public float FastingGlucose { get; set; }

        public float PostPrandialGlucose { get; set; }

        public float MeanBloodGlucose { get; set; }

        public int RiskLabel { get; set; }
    }
}