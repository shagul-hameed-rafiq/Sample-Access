using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class TestSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            // Canonical reference list — validated against WHO/ADA/standard lab references
            var seed = new Dictionary<string, Test>
            {
                // --- Diabetes ---
                // ADA: Normal HbA1c < 5.7%, FBS 70–100 mg/dL, PPBS < 140 mg/dL
                ["DIA_HBA1C"] = new() { TestName = "HbA1c",                   TestCode = "DIA_HBA1C", MinValue = 4.0,    MaxValue = 5.6,    Unit = "%"         },
                ["DIA_FBS"]   = new() { TestName = "FBS",                      TestCode = "DIA_FBS",   MinValue = 70,     MaxValue = 100,    Unit = "mg/dL"     },
                ["DIA_PPBS"]  = new() { TestName = "PPBS",                     TestCode = "DIA_PPBS",  MinValue = 80,     MaxValue = 140,    Unit = "mg/dL"     },
                ["DIA_MBGL"]  = new() { TestName = "Mean Blood Glucose Level", TestCode = "DIA_MBGL",  MinValue = 70,     MaxValue = 150,    Unit = "mg/dL"     },

                // --- Thyroid ---
                // Standard adult reference ranges for TOTAL hormones (not Free)
                // T3 Total: 0.8–2.1 ng/mL  |  T4 Total: 5.0–14.0 µg/dL  |  TSH: 0.4–4.0 mIU/L
                ["THY_TSH"] = new() { TestName = "TSH",       TestCode = "THY_TSH", MinValue = 0.4, MaxValue = 4.0,  Unit = "mIU/L"  },
                ["THY_T3"]  = new() { TestName = "T3 (Total)",TestCode = "THY_T3",  MinValue = 0.8, MaxValue = 2.1,  Unit = "ng/mL"  },
                ["THY_T4"]  = new() { TestName = "T4 (Total)",TestCode = "THY_T4",  MinValue = 5.0, MaxValue = 14.0, Unit = "µg/dL"  },

                // --- CBC ---
                // WHO/standard: General adult ranges (non-gender-specific)
                // Hemoglobin: WHO general adult 12.0–17.5 g/dL (not 12–16 which is female-only)
                ["CBC_HB"]    = new() { TestName = "Hemoglobin",               TestCode = "CBC_HB",    MinValue = 12.0,   MaxValue = 17.5,   Unit = "g/dL"      },
                ["CBC_RBC"]   = new() { TestName = "RBC Count",                TestCode = "CBC_RBC",   MinValue = 4.0,    MaxValue = 6.0,    Unit = "million/µL" },
                ["CBC_WBC"]   = new() { TestName = "WBC Count",                TestCode = "CBC_WBC",   MinValue = 4000,   MaxValue = 11000,  Unit = "cells/µL"  },
                ["CBC_PLT"]   = new() { TestName = "Platelet Count",           TestCode = "CBC_PLT",   MinValue = 150000, MaxValue = 450000, Unit = "cells/µL"  },
                // MCV: corrected typo "MVC" → "MCV"
                ["CBC_MCV"]   = new() { TestName = "MCV",                      TestCode = "CBC_MCV",   MinValue = 80,     MaxValue = 100,    Unit = "fL"        },
                // MCH: corrected unit pg/ml → pg (picograms, not per mL)
                ["CBC_MCH"]   = new() { TestName = "MCH",                      TestCode = "CBC_MCH",   MinValue = 27,     MaxValue = 33,     Unit = "pg"        },
                ["CBC_MCHC"]  = new() { TestName = "MCHC",                     TestCode = "CBC_MCHC",  MinValue = 32,     MaxValue = 36,     Unit = "g/dL"      },
                // RDW: corrected typo "RWD-CV" → "RDW-CV"
                ["CBC_RDW"]   = new() { TestName = "RDW-CV",                   TestCode = "CBC_RDW",   MinValue = 11.5,   MaxValue = 15.0,   Unit = "%"         },
                ["CBC_NEU"]   = new() { TestName = "Neutrophils",              TestCode = "CBC_NEU",   MinValue = 40,     MaxValue = 70,     Unit = "%"         },
                ["CBC_LYM"]   = new() { TestName = "Lymphocytes",              TestCode = "CBC_LYM",   MinValue = 20,     MaxValue = 40,     Unit = "%"         }
            };

            var existingTests = await db.Tests.ToListAsync();

            var toInsert = new List<Test>();
            var updated = false;

            foreach (var entry in seed)
            {
                var testCode   = entry.Key;
                var canonical  = entry.Value;
                var existing   = existingTests.FirstOrDefault(t => t.TestCode == testCode);

                if (existing == null)
                {
                    // New test — queue for insert
                    toInsert.Add(canonical);
                }
                else
                {
                    // Existing test — apply corrections if values differ
                    if (existing.TestName  != canonical.TestName  ||
                        existing.MinValue  != canonical.MinValue  ||
                        existing.MaxValue  != canonical.MaxValue  ||
                        existing.Unit      != canonical.Unit)
                    {
                        existing.TestName = canonical.TestName;
                        existing.MinValue = canonical.MinValue;
                        existing.MaxValue = canonical.MaxValue;
                        existing.Unit     = canonical.Unit;
                        updated = true;
                    }
                }
            }

            if (toInsert.Any())
                await db.Tests.AddRangeAsync(toInsert);

            if (toInsert.Any() || updated)
                await db.SaveChangesAsync();
        }
    }
}
