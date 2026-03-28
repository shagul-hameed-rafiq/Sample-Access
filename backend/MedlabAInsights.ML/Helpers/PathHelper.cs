namespace MedLabAInsights.ML.Helpers
{
    public static class PathHelper
    {
        public static string GetProjectRoot()
        {
            // bin/Debug/net8.0 -> back to project folder
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));
        }

        public static string GetDataFilePath(string fileName)
        {
            return Path.Combine(GetProjectRoot(), "Data", fileName);
        }

        public static string GetModelFilePath(string fileName)
        {
            string modelFolder = Path.Combine(GetProjectRoot(), "MLModels");

            if (!Directory.Exists(modelFolder))
            {
                Directory.CreateDirectory(modelFolder);
            }

            return Path.Combine(modelFolder, fileName);
        }
    }
}