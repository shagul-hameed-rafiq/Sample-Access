using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class PanelTestMappingSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            // Explicit dictionary mapping mapping PanelCode to TestCode & ImportanceLevel
            var mappings = new Dictionary<string, Dictionary<string, int>>
            {
                {
                    "DIA", new Dictionary<string, int>
                    {
                        { "DIA_HBA1C", 1 },
                        { "DIA_FBS", 1 },
                        { "DIA_PPBS", 1 },
                        { "DIA_MBGL", 2 }
                    }
                },
                {
                    "THY", new Dictionary<string, int>
                    {
                        { "THY_TSH", 1 },
                        { "THY_T3",  1 },
                        { "THY_T4",  1 }
                    }
                },
                {
                    "CBC", new Dictionary<string, int>
                    {
                        { "CBC_HB", 1 },
                        { "CBC_RBC", 2 },
                        { "CBC_WBC", 1 },
                        { "CBC_PLT", 1 },
                        { "CBC_MCV", 2 },
                        { "CBC_MCH", 2 },
                        { "CBC_MCHC", 2 },
                        { "CBC_RDW", 2 },
                        { "CBC_NEU", 1 },
                        { "CBC_LYM", 1 }
                    }
                }
            };

            var panels = await db.Panels.ToListAsync();
            var tests = await db.Tests.ToListAsync();
            var newMappings = new List<PanelTestMapping>();

            foreach (var panelMap in mappings)
            {
                var panelCode = panelMap.Key;
                var testDict = panelMap.Value;

                var existingPanel = panels.FirstOrDefault(p => p.PanelCode == panelCode);
                if (existingPanel == null)
                {
                    // Validation: Prevent wrong assignment if Panel doesn't exist
                    Console.WriteLine($"[Warning] Seeder could not find Panel {panelCode}");
                    continue;
                }

                foreach (var testMap in testDict)
                {
                    var testCode = testMap.Key;
                    var importance = testMap.Value;

                    var existingTest = tests.FirstOrDefault(t => t.TestCode == testCode);
                    if (existingTest == null)
                    {
                        // Validation: Prevent mapping if Test doesn't exist
                        Console.WriteLine($"[Warning] Seeder could not find Test {testCode}");
                        continue;
                    }

                    newMappings.Add(new PanelTestMapping
                    {
                        PanelId = existingPanel.PanelId, /* Dynamic Id Assignment */
                        TestId = existingTest.TestId,
                        ImportanceLevel = importance
                    });
                }
            }

            var existingDbMappings = await db.PanelTestMappings.ToListAsync();

            // Identify mappings that are currently in DB but shouldn't be there based on new logic
            var mappingsToRemove = existingDbMappings
                .Where(dbM => !newMappings.Any(newM => newM.PanelId == dbM.PanelId && newM.TestId == dbM.TestId))
                .ToList();

            if (mappingsToRemove.Any())
            {
                db.PanelTestMappings.RemoveRange(mappingsToRemove);
            }

            // Insert only missing mappings
            var toInsert = newMappings
                .Where(newM => !existingDbMappings.Any(dbM => dbM.PanelId == newM.PanelId && dbM.TestId == newM.TestId))
                .ToList();

            if (toInsert.Any())
            {
                await db.PanelTestMappings.AddRangeAsync(toInsert);
            }

            await db.SaveChangesAsync();
        }
    }
}
