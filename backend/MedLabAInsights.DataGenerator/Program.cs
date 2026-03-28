using MedLabAInsights.DataGenerator.Services;

class Program
{
    static void Main(string[] args)
    {
        var generator = new DiabetesSyntheticDataGenerator();

        var rows = generator.Generate(
            normalCount: 700,
            prediabetesCount: 650,
            diabetesCount: 650);

        generator.WriteToCsv("diabetes-training-data.csv", rows);

        Console.WriteLine("Synthetic ML dataset generated successfully.");
    }
}