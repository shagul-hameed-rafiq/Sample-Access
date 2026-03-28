using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Infrastructure;
using MedLabAInsights.Data.Seeding;
using Microsoft.EntityFrameworkCore;

var dbPath = SourcePath.GetFilePath("MedlabAInsight.db");

var options = new DbContextOptionsBuilder<MedlabAinsightDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

await using var db = new MedlabAinsightDbContext(options);

// 1️⃣ Ensure DB schema is up-to-date
await db.Database.MigrateAsync();

// 2️⃣ Seed master data (safe to re-run)
await Seeder.SeedAsync(db);

Console.WriteLine("Database migration and seeding completed.");
