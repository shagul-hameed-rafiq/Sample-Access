using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class PanelSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            var existingCodes = await db.Panels
                .Select(p => p.PanelCode)
                .ToListAsync();

            var panelsToInsert = new List<Panel>
    {
        new Panel { PanelName = "Diabetic", PanelCode = "DIA" },
        new Panel { PanelName = "Thyroid", PanelCode = "THY" },
        new Panel { PanelName = "Complete Blood Count", PanelCode = "CBC" }
    }
            .Where(p => !existingCodes.Contains(p.PanelCode))
            .ToList();

            if (!panelsToInsert.Any())
                return;

            await db.Panels.AddRangeAsync(panelsToInsert);
            await db.SaveChangesAsync();
        }

    }
}
