using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MedLabAInsights.Data.Infrastructure;

namespace MedLabAInsights.Data.Contexts
{
    public class MedlabAInsightDbContextFactory : IDesignTimeDbContextFactory<MedlabAinsightDbContext>
    {
        public MedlabAinsightDbContext CreateDbContext(string[] args)
        {
            var dbPath = SourcePath.GetFilePath("MedlabAInsight.db");
            var optionsBuilder = new DbContextOptionsBuilder<MedlabAinsightDbContext>()
                .UseSqlite($"Data Source={dbPath}");

            return new MedlabAinsightDbContext(optionsBuilder.Options);
        }
    }
}
