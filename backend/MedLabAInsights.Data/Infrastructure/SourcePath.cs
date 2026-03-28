namespace MedLabAInsights.Data.Infrastructure
{
    public static class SourcePath
    {
        public static string GetFilePath(string fileName)
        {
            // AppContext.BaseDirectory -> .../bin/Debug/net8.0/
            // Move up 4 levels to reach project folder
            var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            return Path.Combine(projectDir, "MedLabAInsights.Data", fileName);
        }
    }
}
